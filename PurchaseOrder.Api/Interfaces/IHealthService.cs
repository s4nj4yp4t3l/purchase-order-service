using PurchaseOrder.Api.Concrete;

namespace PurchaseOrder.Api.Interfaces
{
    /// <summary>
    /// Interface for the <see cref="HealthService"/>
    /// </summary>
    public interface IHealthService
    {
        /// <summary>
        /// Get a health status of the API returing a type <see cref="System.String"/>.
        /// </summary>
        /// <returns>Asnychronous object of type <see cref="IResult"/>.</returns>
        Task<IResult> GetHealthStatusAsync();
    }
}