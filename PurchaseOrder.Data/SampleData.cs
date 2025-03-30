using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Models.Constants;

namespace PurchaseOrder.Data
{
    /// <summary>
    /// Just some properties to get some hard coded data as we are not using a database in this application.
    /// This data is used in the API itself and also in the unit tests.
    /// </summary>
    public static class SampleData
    {
        /// <summary>
        /// Hard coded <see cref="List{Item}}"/>.
        /// </summary>
        public static List<Item> Items
        {
            get
            {
                return
                [
                    new() { Id = 1, Title = Membership.BOOK_CLUB_MEMBERSHIP, Price = 1.11M, IsPhysicalItem = false, },
                    new() { Id = 2, Title = Membership.VIDEO_CLUB_MEMBERSHIP, Price = 2.22M, IsPhysicalItem = false, },
                    new() { Id = 3, Title = "Aliens - Special Edition", Price = 3.33M, IsPhysicalItem = true },
                    new() { Id = 4, Title = "Star Wars - A New Hope", Price = 4.44M, IsPhysicalItem = true },
                    new() { Id = 5, Title = "C# 13 For Professionals", Price = 5.55M, IsPhysicalItem = true },
                    new() { Id = 6, Title = "HTML 5 For Beginners", Price = 6.66M, IsPhysicalItem = true }
                ];
            }
        }

        /// <summary>
        /// Hard coded <see cref="List{PurchaseOrderResponse}}"/>.
        /// </summary>
        public static List<PurchaseOrderResponse> PurchaseOrderResponses
        {
            get
            {
                return
                [
                    new PurchaseOrderResponse() { PoId = 1, CustomerId = 1, Items = [Membership.BOOK_CLUB_MEMBERSHIP, "Aliens - Special Edition"], Total = 44.44M },
                    new PurchaseOrderResponse() { PoId = 2, CustomerId = 2, Items = [Membership.VIDEO_CLUB_MEMBERSHIP, "Star Wars - A New Hope", "HTML 5 For Beginners"], Total = 13.32M },
                    new PurchaseOrderResponse() { PoId = 3, CustomerId = 3, Items = ["Aliens - Special Edition", "C# 13 For Professionals" ], Total = 8.88M },
                    new PurchaseOrderResponse() { PoId = 4, CustomerId = 4, Items = ["Star Wars - A New Hope", "C# 13 For Professionals", "HTML 5 For Beginners" ], Total = 16.65M },
                    new PurchaseOrderResponse() { PoId = 5, CustomerId = 5, Items = [Membership.BOOK_CLUB_MEMBERSHIP, Membership.VIDEO_CLUB_MEMBERSHIP, "HTML 5 For Beginners", "Aliens - Special Edition", ], Total = 13.32M },
                ];
            }
        }

        /// <summary>
        /// Hard coded <see cref="PurchaseOrderRequest"/>.
        /// </summary>
        public static PurchaseOrderRequest PurchaseOrderRequest
        {
            get
            {
                List<int> sampleIds = [1, 2, 3, 6];
                List<Item> sampleItems = [.. SampleData.Items.Where(x => sampleIds.Contains(x.Id))];
                return new() { 
                    CustomerId = 5,
                    // Matches PurchaseOrderResponses where PoId = 5.
                    Items = sampleItems,
                };  
            }
        }
    }
}