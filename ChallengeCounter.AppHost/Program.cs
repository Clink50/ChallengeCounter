var builder = DistributedApplication.CreateBuilder(args);

// var sql = builder.AddPostgres("sql", port: 5432)
//     .WithImageTag("latest")
//     .WithEnvironment("POSTGRES_DB", "challengecounter")
//     .WithDataVolume();

// var db = sql.AddDatabase("challengecounter");

var api = builder.AddProject<Projects.ChallengeCounter_Api>("api");
    // .WithReference(db).WaitFor(db);

var web = builder.AddNpmApp("web", "../ChallengeCounter.Client")
    .WithNpmPackageInstallation()
    .WithHttpEndpoint(env: "PORT")
    .WithOtlpExporter()
    .WithExternalHttpEndpoints()
    .WithReference(api).WaitFor(api);

builder.Build().Run();
