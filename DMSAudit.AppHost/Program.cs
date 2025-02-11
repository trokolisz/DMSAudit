var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DMSAudit_ApiService>("apiservice");

builder.AddProject<Projects.DMSAudit_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
