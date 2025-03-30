using Microsoft.Extensions.Logging;
using PurchaseOrder.Data;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Repository.Interfaces;

namespace PurchaseOrder.Repository.Concrete
{
    public class PurchaseOrderRepository(ILogger<PurchaseOrderRepository> logger) : IPurchaseOrderRepository
    {
        private readonly ILogger<PurchaseOrderRepository> _logger = logger;

        // This would normally be a table in the database.
        // Assume "PoId" is PK and therefore unique.
        private readonly List<PurchaseOrderResponse> _purchaseOrderResponses = SampleData.PurchaseOrderResponses;

        /// <inheritdoc/>
        public async Task<PurchaseOrderResponse?> GetPurchaseOrderAsync(int poId)
        {
            // Usually this would be a call to the data source
            await Task.Delay(1); // EF context call would be async.
            _logger.LogInformation("Running 'GetPurchaseOrderAsync'");

            PurchaseOrderResponse? purchaseOrderResponse = _purchaseOrderResponses.Where(c => c.PoId == poId).FirstOrDefault();

            if (purchaseOrderResponse == null)
                _logger.LogInformation("No matching purchase order found for {poId}", poId);
            else
                _logger.LogInformation("Matching purchase order found for {poId}", poId);

            return purchaseOrderResponse;
        }

        /// <inheritdoc/>
        public async Task<PurchaseOrderResponse?> ProcessPurchaseOrderAsync(
            PurchaseOrderRequest purchaseOrderRequest,
            List<string> customerMembershipTypes,
            List<int> physicalItemIds)
        {
            // Usually this would be a call to the data source
            await Task.Delay(1); // EF context call would be async.
            _logger.LogInformation("Running 'ProcessPurchaseOrderAsync'");
            _logger.LogInformation("With purchase order request parameter: {purchaseOrderRequest}'", Newtonsoft.Json.JsonConvert.SerializeObject(purchaseOrderRequest));
            _logger.LogInformation("With customer membership types parameter: {customerMembershipTypes}'", string.Join(" ", customerMembershipTypes));
            _logger.LogInformation("With physical item id's parameter: '{physicalItemIds}'", string.Join(", ", physicalItemIds));

            // If using an ADO.NET transaction set one using "System.Data.SqlClient.SqlTransaction", use the
            // "System.Data.SqlClient.SqlCommand", run our 3 writes, then if all ok "Commit()", else "Rollback()".
            // If using EF transaction set one using "System.Data.Common.BeginTransactionAsync", run our 3 writes,
            // calling "context.SaveChangesAsync()" on each. Then do "CommitAsync();" if all ok.
            // OR....call a sproc which has the transaction code in SQL.

            // Write 1: Update the customer. BR1 - If the purchase order contains a membership, it has to be
            // activated in the customer account immediately. 
            // As "customer" isn't part of the "order" domain, we can't update it directly. Instead, we would send 
            // the customer update as a JSON object to a Service Bus queue using NuGet "Azure.Messaging.ServiceBus".
            // Then a subscriber - An Azure Function App say, would read / consume that message, process it by
            // modifying the customer table with the memberships. The same Azure Function App could then send
            // a message to a different queue to say the update was done in the "customer" domain. We could read
            // the message here and know if all was OK. As using the Service Bus to send/receive messages from queue,
            // probably best to package this into a company NuGet to abtract out the complexity (although it isn't
            // hard code using "Azure.Messaging.ServiceBus") from this API.
            _logger.LogInformation("Modifying customer with id: {customerId} with membership types {customerMembershipTypes}", 
                purchaseOrderRequest.CustomerId,
                string.Join(" ", customerMembershipTypes));

            // Write 2: Add the shipping slip. BR2 - If the purchase order contains a physical product a shipping
            // slip has to be generated.
            // This is similar to "Write 1". The "shipping" domain isn't part of the "order" domain, so we would
            // liaise with the "shipping" domain via Service Bus queues.
            _logger.LogInformation("Adding shipping slip for customer id: {customerId} with item id's: {physicalItemIdsCommaDelimeted}", purchaseOrderRequest.CustomerId, string.Join(", ", physicalItemIds));

            // Write 3: Add the purchase order.
            // This would be a straight-forward database write within our domain.
            _logger.LogInformation("Adding purchase order for customer id: {customerId}", purchaseOrderRequest.CustomerId);

            // The query below is just matching on purchase order id. In a real scenario with a proper database,
            // a "PurchaseOrderResponse" would be created and returned back...not a hard-coded one with PoId = 1.
            return _purchaseOrderResponses.Where(x => x.PoId == 5).FirstOrDefault()!;
        }
    }
}