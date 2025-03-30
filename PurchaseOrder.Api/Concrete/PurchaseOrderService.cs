using FluentValidation;
using PurchaseOrder.Api.Interfaces;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Models.Constants;
using PurchaseOrder.Models.Enums;
using PurchaseOrder.Repository.Interfaces;

namespace PurchaseOrder.Api.Concrete
{
    /// <summary>
    /// Service class to work with the purchase order endpoint.
    /// </summary>
    /// <param name="logger">The injected <see cref="ILogger{PurchaseOrderService}"/> logger.</param>
    /// <param name="validator">The injected <see cref="IValidator{PurchaseOrderRequest}"/> validator.</param>
    /// <param name="purchaseOrderRepository">The injected <see cref="IPurchaseOrderRepository"/> repository.</param>
    public class PurchaseOrderService(
        ILogger<PurchaseOrderService> logger,
        IValidator<PurchaseOrderRequest> validator,
        IPurchaseOrderRepository purchaseOrderRepository
        ) : IPurchaseOrderService
    {
        private readonly ILogger<PurchaseOrderService> _logger = logger;
        private readonly IValidator<PurchaseOrderRequest> _validator = validator;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository = purchaseOrderRepository;

        /// <inheritdoc/>
        public async Task<IResult> GetPurchaseOrderAsync(int poId)
        {
            PurchaseOrderResponse? purchaseOrderResponse = await _purchaseOrderRepository.GetPurchaseOrderAsync(poId);

            if (purchaseOrderResponse is null)
            {
                // If PurchaseOrderResponse not found send back a HTTP 404.
                _logger.LogWarning("No purchase order found for id {id}", poId);
                return Results.NotFound($"Purchase order with id {poId} not found");
            }

            // If PurchaseOrderResponse found send back a HTTP 200.
            _logger.LogInformation("Purchase order found with id {poid}", poId);
            return Results.Ok(purchaseOrderResponse);
        }

        /// <inheritdoc/>
        public async Task<IResult> ProcessPurchaseOrderAsync(PurchaseOrderRequest purchaseOrderRequest)
        {
            try
            {
                // Check if the passed in modal is valid.
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(purchaseOrderRequest);

                if (!validationResult.IsValid)
                {
                    // If validation errors send back a HTTP 400.
                    _logger.LogWarning("Validation failure: {validationResult}", string.Join(" ", validationResult.Errors));
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                // We wouldn't need to check if the customer exist as they would have just placed this order
                // so unlikely to have been removed from the system in that very short space of time.

                // We wouldn't need to check if any items in the list exist as the customer would have just placed
                // this order so unlikely to have been removed from the system in that very short space of time.

                // Add any memberships to the customer if they exist in the items collection.
                List<string> membershipTypes = [];
                bool bookClubMembershipExists = purchaseOrderRequest.Items.Where(x => x.Title.Contains(Membership.BOOK_CLUB_MEMBERSHIP)).Any();
                bool videoClubMembershipExists = purchaseOrderRequest.Items.Where(x => x.Title.Contains(Membership.VIDEO_CLUB_MEMBERSHIP)).Any();

                if (bookClubMembershipExists)
                    membershipTypes.Add(nameof(MembershipTypeEnum.MembershipType.Book));

                if (videoClubMembershipExists)
                    membershipTypes.Add(nameof(MembershipTypeEnum.MembershipType.Video));

                // Get a list of physical item id's that need to have a shipping slip.
                List<int> physicalItemIds = [.. purchaseOrderRequest.Items.Where(x => x.IsPhysicalItem == true).Select(y => y.Id)];

                // This will be our response object after a successful call.
                // We are doing 3 writes to that must have atomicity.
                PurchaseOrderResponse? purchaseOrderResponse = await _purchaseOrderRepository.ProcessPurchaseOrderAsync(
                    purchaseOrderRequest, membershipTypes, physicalItemIds);

                if (purchaseOrderResponse == null)
                    // Use 422 - The request was well-formed (i.e., syntactically correct) but could not be processed.
                    return Results.UnprocessableEntity("Purchase order could not be processed");

                // The "purchaseOrderResponse" is a fake one as we are using dummy data. So to match what we 
                // have requested, I'm setting the values again below.
                purchaseOrderResponse.PoId = new Random().Next(10, 100); // Just any random integer between 10 and 99.;
                purchaseOrderResponse.Items = [.. purchaseOrderRequest.Items.Select(x => x.Title)];
                purchaseOrderResponse.Total = purchaseOrderRequest.Items.Sum(s => s.Price);
                purchaseOrderResponse.CustomerId = purchaseOrderRequest.CustomerId;

                _logger.LogInformation("Purchase order created {purchaseOrderResponse}",
                    Newtonsoft.Json.JsonConvert.SerializeObject(purchaseOrderResponse));

                // Use 201 - Created specifically for successful creation operations.
                return Results.Created($"/api/purchaseorder/{purchaseOrderResponse.PoId}", purchaseOrderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: {exception}", ex);
                // Use 422 - The request was well-formed (i.e., syntactically correct) but could not be processed.
                return Results.UnprocessableEntity("Exception occurred");
            }
        }
    }
}