using PurchaseOrder.Api.Concrete;
using PurchaseOrder.Models.Concrete;

namespace PurchaseOrder.Api.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="PurchaseOrderService"/>
    /// </summary>
    public interface IPurchaseOrderService
    {
        /// <summary>
        /// Get a <see cref="PurchaseOrderResponse"/> for a given purchase order id.
        /// </summary>
        /// <param name="poId">The purchase order id.</param>
        /// <returns>Asnychronous object of type <see cref="IResult"/>.</returns>
        Task<IResult> GetPurchaseOrderAsync(int poId);

        /// <summary>
        /// Process a purchase order which is a 3 step process; add a purchase
        /// order, update the customer and add a shipping slip. The last 2 are optional depending on the
        /// data supplied.
        /// </summary>
        /// <param name="purchaseOrderRequest">An object of type <see cref="PurchaseOrderRequest"/>.</param>
        /// <returns>Asnychronous object of type <see cref="IResult"/>.</returns>
        Task<IResult> ProcessPurchaseOrderAsync(PurchaseOrderRequest purchaseOrderRequest);
    }
}