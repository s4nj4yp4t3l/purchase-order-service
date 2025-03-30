using Microsoft.AspNetCore.Mvc;
using PurchaseOrder.Api.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace PurchaseOrder.Api.Endpoints
{
    /// <summary>
    /// Define the health endpoints.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public static class HealthEndpoints
    {
        public static void AddHealthEndpoints(this WebApplication app)
        {
            // Get health.
            // You could hard code the version into the route, e.g. "/api/v1/health", however, it is better
            // to use APIM versioning.
            app.MapGet("/api/health", async ([FromServices] IHealthService healthService) =>
                await healthService.GetHealthStatusAsync())
                .Produces(200) // OK.
                .WithName("Get health") // Used by LinkGenerator to create Url's & create unique operationId in Open API spec.
                .WithTags("heathcheck") // Group related operations.
                .WithDescription("Get health to check "); // Description for this endpoint in Open API spec.
        }
    }
}