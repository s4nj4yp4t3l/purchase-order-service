namespace PurchaseOrder.Models.Enums
{
    /// <summary>
    /// Enumeration for membershiip type.
    /// </summary>
    public class MembershipTypeEnum
    {
        /// <summary>
        /// A customer can have either "book", "video" or both memberships.
        /// </summary>
        public enum MembershipType
        {
            Book,
            Video
        }
    }
}