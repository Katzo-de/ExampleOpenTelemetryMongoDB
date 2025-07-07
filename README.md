# Example OpenTelemetry MongoDB
This repository contains a sample project to demonstrate distributed tracing, metrics and logging with OpenTelemetry in a modern .NET ASP.NET Core application with MongoDB connectivity.
The goal of the project is to show the integration and use of OpenTelemetry in realistic .NET APIs - including automatic and manual instrumentation for traces, structured logs and application metrics.

## Dependencies
 Install the following dependencies:
- Docker Desktop
- Visual Studio 

## Setup Aspire and MongoDB
 1. Start Aspire using the following command:
	```powershell
	docker run --rm -it -d -p 18888:18888 -p 4317:18889 -e ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:latest
	```
 2. Install and Start [MongoDB](https://www.mongodb.com/docs/manual/tutorial/install-mongodb-community-with-docker/#procedure) using follwing commands:
    ```powershell
    docker pull mongodb/mongodb-community-server:latest
    docker run --name mongodb -p 27017:27017 -d mongodb/mongodb-community-server:latest
    ```







---
# Example Create Span

## Example Usage with Span/Activities in Threads
```csharp
var parent = Activity.Current;
var thread = new Thread(() =>
{
    Activity.Current = parent;
    using var span = ActivitySource.StartActivity("ChildInThread");
    // ...  
});
thread.Start();
```
```csharp
var parent = Activity.Current;
Parallel.ForEach(items, item =>
{
    // Parent-Context setzen (wichtig!)
    Activity.Current = parent;

    using var span = ActivitySource.StartActivity("ParallelItem", ActivityKind.Internal);
    span?.SetTag("item.value", item);

    // ... Arbeit
});
```