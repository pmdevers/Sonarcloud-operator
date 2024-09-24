using K8sOperator.NET;
using K8sOperator.NET.Builder;
using K8sOperator.NET.Extensions;
using K8sOperator.NET.Generators;
using Sonarcloud.Commands;
using Sonarcloud.Projects;
using SonarCloud.NET.Extensions;

var builder = OperatorHost.CreateOperatorApplicationBuilder(args);

builder.Configuration.AddUserSecrets<ProjectsController>();

builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddSonarCloudClient(options =>
{
    builder.Configuration.Bind("SonarCloud", options);
});

builder.AddController<ProjectsController>()
    .WithFinalizer("projects.sonarcloud.io");

var host = builder.Build();

host.AddInstall();
host.AddCommand<CreateCommand>();

await host.RunAsync();

