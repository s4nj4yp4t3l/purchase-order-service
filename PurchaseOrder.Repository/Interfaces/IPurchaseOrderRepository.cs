using PurchaseOrder.Models.Concrete;

namespace PurchaseOrder.Repository.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        /// <summary>
        /// Get a <see cref="PurchaseOrderResponse?"/> given a customer id.
        /// </summary>
        /// <remarks>
        /// Usually this would be a call to the data source.
        /// </remarks>
        /// <param name="poId">The purchase order id.</param>
        /// <returns>Asnychronous object of type <see cref="PurchaseOrderResponse?"/>.</returns>
        Task<PurchaseOrderResponse?> GetPurchaseOrderAsync(int poId);

        /// <summary>
        /// Add a <see cref="PurchaseOrderRequest"/>.
        /// </summary>
        /// <remarks>
        /// Usually this would be a call to the data source. As we have a series of writes to the database; 
        /// We should wrap these in a transaction so we have some atomicity in the database. Its quite easy 
        /// to do this using either AD.NET or EF.
        /// </remarks>
        /// <param name="purchaseOrderRequest">The purchase order request.</param>
        /// <param name="customerMembershipTypes">The customer membership types.</param>
        /// <param name="physicalItemIds">The list of physical item id's to ship to the customer.</param>
        /// <returns>Asnychronous object of type <see cref="PurchaseOrderResponse"/> or null if there 
        /// was a problem with the transactional writes.</returns>
        Task<PurchaseOrderResponse?> ProcessPurchaseOrderAsync(
            PurchaseOrderRequest purchaseOrderRequest,
             List<string> customerMembershipTypes,
             List<int> physicalItemIds);
    }
}