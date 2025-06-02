var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
    .WithImage("postgres:13-alpine")
    .WithDataBindMount(source: @".\PostgreSQL\Data", isReadOnly: false)
    .WithPgAdmin();
var postgresdb = postgres.AddDatabase("event-sourcing");


// Register the API project and connect it to the Postgres database
builder.AddProject<Projects.EventSourcing>("api")
    .WithReference(postgresdb, "DbConnection")
    .WaitFor(postgresdb);

builder.Build().Run();
