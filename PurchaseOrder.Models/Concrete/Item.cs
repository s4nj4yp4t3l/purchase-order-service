namespace PurchaseOrder.Models.Concrete
{
    /// <summary>
    /// POCO for <see cref="Item"/>.
    /// </summary>
    /// <remarks>
    /// This POCO is just ba helper for my sample data that I use in my unit tests. It's not used in the API at all.
    /// </remarks>
    public class Item 
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required decimal Price { get; set; }
        public required bool IsPhysicalItem { get; set; }
    }
}