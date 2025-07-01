var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ChallengeCounter_Api>("api");

builder.AddNpmApp("web", "../ChallengeCounter.Client")
    .WithNpmPackageInstallation()
    .WithHttpEndpoint(env: "PORT")
    .WithOtlpExporter()
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WithReference(api).WaitFor(api);

builder.Build().Run();
