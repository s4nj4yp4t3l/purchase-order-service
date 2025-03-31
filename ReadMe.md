# FunBooksAndVideos e-commerce shop

## Datasource

There is no data source in this solution. I've tried to keep it simple by having repository classes that just use 
hard coded data. By doing this, the data isn't 100% correct, but this isn't a requirement.

## How to call the API

To test the services, to help, I've put some examples in the "PurchaseOrder.Api.http" file, which is part of the 
"PurchaseOrder.Api" project.

Some sample data can be found in "PurchaseOrder.Data" project in the "SampleData.cs" file.

This way you can see the data that can be used to make successful / unsuccessful calls to the "ProcessPurchaseOrderAsync" 
HTTP POST route of "/api/purchaseorder".

## The code itself

### Coding principles

I've used; OO design principles, DRY design principles, KISS design principles, SOLID design principles as per the standard
when writing applications.

By using proper OO & SOLID design principles we can achieve absolute code coverage. The code coverage in this solution 
is 100%. You can check this in Visual Studio. Go to the "Test" menu item, then choose "Analyze Code Coverage for All Tests".

I've injected the "Microsoft.Extentions.Logging.ILogger" into most classes so I can log message. This helps with debugging 
any issues in the cloud with Application Insights, where you don't have access to a local debugger like Visual Studio.

As per some company standards, I've included a health service endpoint. This is just a very simple HTTP GET that returns
a string to indicate that the API is working. It's useful when an API is deployed to the cloud as a quick test to see if
its functioning.

### Naming standards

As per the standards (at most companies I've worked at) the name of the solution is based on what the API does. 
This is a purchase order API, so the solution is called "purchase-order-service". It is then split into the 
solution folders, "Api", "Common", "Test" for clarity. We have the single API project named after
what the API does, so "PurchaseOrder.Api" - purchase order being what part of the solution name is. 
Then we have the class library projects "PurchaseOrder.Models", "PurchaseOrder.Repository" and "PurchaseOrder.Data". 
If the models project if shared between other solution it could be a NuGet package instead. If we had a domain
project "PurchaseOrder.Domain" it would hold the functionality for database interaction separately. If (and I haven't 
done this as i've hard coded the data) I was connecting to a database, say using Entity Framework Core, then I would
have a "PurchaseOrder.Domain" project that would contain a "ProcessOrderContext" class inheriting the 
"Microsoft.EntityFrameworkCore.DbContext" class. The "PurchaseOrder.Data" project is only needed because it acts as 
my data source as mentioned before, there is no database in this solution.

Finally, we have the test projects. There is one for each other project in the solution, so 
"PurchaseOrder.Api.UnitTests", "PurchaseOrder.Repository.UnitTests". The "PurchaseOrder.Models" doesn't need code 
coverage as its just POCO's.

Each unit test project is named the same as the project it is applying tests to, with the suffix ".UnitTests". 
Also, tests are called after the class they are targeting with the suffix "Tests", e.g. for "PurchaseOrderService.cs" 
we have "PurchaseOrderServiceTests.cs". We should also mimic the folder structure so its easy to locate the 
required tests.

For the unit tests I've used the NuGet's Xunit, Moq & FluentAssertions. You could use the standard Xunit "Assert" 
class (instead of FluentAssertions) it will accomplish the same goal, it s just a preference / company coding standard.

### Asynchronous methods

I've used the "async" in the method definition and hence some calls in the method are "await". This is better in an API
as it allows other tasks to continue while maybe waiting for one to finish. Also, threads can be free to do other work and
not be blocked. Our API would be making intensive calls to a database if there was one hooked into this solution. Hence, it 
is better to make our API endpoints and the methods connected to it in the chain asynchronous. Any calling code
in the UI can call our endpoint using the "HttpClientFactory" asynchronously as well.

### New changes to .NET 9.0

I've written the solution in .NET 9.0, the latest version at the time of writing. I've also used a minimal API rather than
the controller-based option. Minimal APIs are generally faster than Controller APIs because they have less overhead, 
especially for simple operations. As the complexity increases though the performance difference fall. As APIs should
be simple as per a Microservice, I've used Minimal APIs.

New to .NET core 9.0 is that Swagger has been removed - see https://github.com/dotnet/aspnetcore/issues/54599. 
So I m using the Scalar NuGet instead, see https://www.nuget.org/packages/Scalar.AspNetCore/. It works in a 
similar way to Swagger. The Scalar UI route is detailed in the "Program.cs" file.

### "Customer" and "Shipping" domains

Now this API is in the "Order" domain. The specification requirement was to create a purchase order AND apply any memberships 
to the customer and also send a shipping slip for physical items. The customer would be in the "Customer" domain and 
the shipping slip creation would be in the "Shipping" domain. To talk to other domains using event driven design, we use 
a Service Bus to send and receive messages. This means the domains are de-coupled as per microservice EDD best 
practices. I've detailed in the "ProcessPurchaseOrderAsync" method in the "PurchaseOrderRepository.cs" file in the 
"PurchaseOrder.Repository" project what this entails.

## Microservice

A microservice should be entirely independent from other microservices and individually deployable. Now I'd assume that
the doamin for this "purchase-order-service" microservice is "orders" which deal's with everything order 
related. That is why is only has the endpoints in "PurchaseOrderEndpoints.cs". To communicate across domains we use
Service Buses. We talk to the "customer" domain to update a customer's membership or talk to the "shipping" domain
to send out shipping-slips. We'd do this via messages in queues. This means different parts of the overall system 
(the domains) are loosly coupled meaning a more independent, scalable system.

## Containerization.

To be a proper (although apparently its not a strict requirement) microservice, we containerise this API so it can easily be 
scaled. We would add a Dockerfile, to "Dockerise" this solution (including running the unit tests). Then we can create steps in 
this Dockerfile to create the image for this API. Locally we can use Docker Desktop to test this. Finally, when happy, we could
save this Dockerised solution in a registry like DockerHub or Azure Container Registry. Then we can create an Azure Container 
App (ACA) that links to our image. Using ACA we can do many things, set resiliency, better scalability, easier deployment etc. 
After that we can include our ACA API in Azure APIM using the Open API JSON. The APIM can control versioning, caching & have
better / central security, good analytics etc. Better to use APIM versioning than hard code a version number into the API route.
