namespace PurchaseOrder.Models.Concrete
{
    /// <summary>
    /// DTO for <see cref="PurchaseOrderRequest"/>.
    /// </summary>
    public class PurchaseOrderRequest
    {
        public required int CustomerId { get; set; }
        // We'll assume that the request can only contain one "book" membership and/or one "video" membership.
        public required List<Item> Items { get; set; }
    }
}