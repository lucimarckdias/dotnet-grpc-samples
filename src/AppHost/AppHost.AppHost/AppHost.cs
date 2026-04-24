var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.GrpcServer>("grpc-server");
var client = builder.AddProject<Projects.GrpcClient>("grpc-client")
    .WithReference(server);

builder.Build().Run();
