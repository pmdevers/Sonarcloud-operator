using k8s;
using K8sOperator.NET;
using K8sOperator.NET.Builder;
using K8sOperator.NET.Extensions;
using K8sOperator.NET.Generators;
using Sonarcloud.Commands;
using Sonarcloud.Projects;
using SonarCloud.NET.Extensions;

if (KubernetesClientConfiguration.IsInCluster())
{
    Console.WriteLine("Running in cluster with config: ");
    var config = KubernetesClientConfiguration.InClusterConfig();

    Console.WriteLine("Host: '{0}'", config.Host);
    Console.WriteLine("Namespace: '{0}'", config.Namespace);
    Console.WriteLine("TlsServerName: '{0}'", config.TlsServerName);
    Console.WriteLine("Username: '{0}'", config.Username);
    Console.WriteLine("Password: '{0}'", config.Password); 
    Console.WriteLine("UserAgent: '{0}'", config.UserAgent);



    Console.WriteLine();
    Console.WriteLine("########################");
}


var builder = OperatorHost.CreateOperatorApplicationBuilder(args)
    .WithName("sonarcloud-operator")
    .WithImage(registery: "ghcr.io", repository: "pmdevers", name: "sonarcloud-operator", "1.0.0");

builder.Configuration.AddUserSecrets<ProjectsController>();

builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddSonarCloudClient(options =>
{
    builder.Configuration.Bind("SonarCloud", options);
});

builder.AddController<ProjectsController>()
    .WatchNamespace(builder.Configuration.GetValue<string>("NAMESPACE") ?? "default")
    .WithFinalizer("projects.sonarcloud.io");

var host = builder.Build();

host.AddInstall();
host.AddCommand<CreateCommand>();

await host.RunAsync();

