namespace PurchaseOrder.Models.Concrete
{
    /// <summary>
    /// DTO for <see cref="PurchaseOrderResponse"/>.
    /// </summary>
    public class PurchaseOrderResponse()
    {
        public int PoId { get; set; }
        public int CustomerId { get; set; }
        public List<string> Items { get; set; } = []; // From the spec we just need the title, hence System.String.
        public decimal Total { get; set; }
    }
}