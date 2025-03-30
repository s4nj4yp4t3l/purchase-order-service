using Microsoft.AspNetCore.Mvc;
using PurchaseOrder.Api.Interfaces;
using System.Diagnostics.CodeAnalysis;
using PurchaseOrder.Models.Concrete;

namespace PurchaseOrder.Api.Endpoints
{
    /// <summary>
    /// Define the purchase order endpoints.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public static class PurchaseOrderEndpoints
    {
        public static void AddPurchaseOrderEndpoints(this WebApplication app)
        {
            // Add a purchase order.
            // You could hard code the version into the route, e.g. "/api/v1/purchaseorder", however, it is better
            // to use APIM versioning.
            app.MapPost("/api/purchaseorder", async ([FromServices] IPurchaseOrderService purchaseOrderService, [FromBody] PurchaseOrderRequest purchaseOrderRequest) =>
            await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest))
                .Accepts<PurchaseOrderRequest>(false, "application/json")
                .Produces(201) // Created.
                .Produces(400) // Bad Request.
                .Produces(404) // Not Found.
                .Produces(422) // Unprocessable Content.
                .WithName("Add a purchase order") // Used by LinkGenerator to create Url's & create unique operationId in Open API spec.
                .WithTags("PurchaseOrder") // Group related operations.
                .WithDescription("Add a purchase order"); // Description for this endpoint in Open API spec.

            // Get a purchase order.
            // You could hard code the version into the route, e.g. "/api/v1/purchaseorder/1", however, it is better
            // to use APIM versioning.
            app.MapGet("/api/purchaseorder/{poId}", async ([FromServices] IPurchaseOrderService purchaseOrderService, int poId) =>
                await purchaseOrderService.GetPurchaseOrderAsync(poId))
                .Produces(200) // OK.
                .Produces(404) // Not Found.
                .WithName("Get a purchase order") // Used by LinkGenerator to create Url's & create unique operationId in Open API spec.
                .WithTags("PurchaseOrder") // Group related operations.
                .WithDescription("Get a purchase order"); // Description for this endpoint in Open API spec.
        }
    }
}