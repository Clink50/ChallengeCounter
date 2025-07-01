var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddPostgres("sql", port: 5432)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("latest")
    .WithDataVolume();

var db = sql.AddDatabase("challengecounter");

var api = builder.AddProject<Projects.ChallengeCounter_Api>("api")
    .WithReference(db).WaitFor(db);

builder.AddNpmApp("web", "../ChallengeCounter.Client")
    .WithNpmPackageInstallation()
    .WithHttpEndpoint(env: "PORT")
    .WithOtlpExporter()
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WithReference(api).WaitFor(api);

builder.Build().Run();
