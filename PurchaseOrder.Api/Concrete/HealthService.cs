using PurchaseOrder.Api.Interfaces;

namespace PurchaseOrder.Api.Concrete
{
    /// <summary>
    /// Service class to work with the health endpoint.
    /// </summary>
    /// <param name="logger">The injected <see cref="ILogger{HealthService}"/> logger.</param>
    public class HealthService(ILogger<HealthService> logger) : IHealthService
    {
        private readonly ILogger _logger = logger;

        /// <inheritdoc/>
        public async Task<IResult> GetHealthStatusAsync()
        {
            _logger.LogInformation("Returning health status");
            await Task.Delay(1);
            return Results.Ok($"Healthy @{DateTime.Now}");
        }
    }
}