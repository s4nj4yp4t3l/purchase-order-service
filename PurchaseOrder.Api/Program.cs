using FluentValidation;
using PurchaseOrder.Api.Concrete;
using PurchaseOrder.Api.Endpoints;
using PurchaseOrder.Api.Interfaces;
using PurchaseOrder.Api.Validations;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Repository.Concrete;
using PurchaseOrder.Repository.Interfaces;
using Scalar.AspNetCore;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// We no longer have swagger, instead we use the open api package.
builder.Services.AddOpenApi();

builder.Services.AddApplicationInsightsTelemetry(); // Azure Application Insights for monitoring.

// For endpoint services in a web application context, scoped services are generally preferred as they ensure a new instance for each HTTP request.
builder.Services.AddScoped<IHealthService, HealthService>(); // Method for our single HTTP GET endpoint.
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>(); // Various methods for our HTTP GET | POST.

builder.Services.AddScoped<IValidator<PurchaseOrderRequest>, PurchaseOrderRequestValidator>(); // Fluent Validation DI.

builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>(); // Repository for purchase orders.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 0000 is the port in your launchSettings.json > "applicationUrl". If your using HTTPS redirection
    // then even if you use the port of HTTP, it will redirect you to the port using HTTPS.

    // Browse to the default https://localhost:7158/openapi/v1.json to se the open API definition.
    app.MapOpenApi();
    // Browse to the default https://localhost:7158/scalar/v1 to see a UI. 
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.AddHealthEndpoints();
app.AddPurchaseOrderEndpoints();

app.Run();

// To exclude this file from code coverage, we have to define a partial class to add the attribute to.
[ExcludeFromCodeCoverageAttribute]
public partial class Program { }
