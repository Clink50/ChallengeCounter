var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddPostgres("sql")
    .WithImageTag("latest")
    .WithEnvironment("POSTGRES_DB", "challengecounter")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

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
