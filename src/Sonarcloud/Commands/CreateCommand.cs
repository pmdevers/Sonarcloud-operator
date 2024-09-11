using k8s;
using K8sOperator.NET;
using K8sOperator.NET.Builder;
using K8sOperator.NET.Metadata;
using Sonarcloud.Projects;

namespace Sonarcloud.Commands;

[OperatorArgument("create", Description = "Create a custom resource definition.")]
public class CreateCommand(IOperatorApplication app) : IOperatorCommand
{
    public Task RunAsync(string[] args)
    {
        if(args.Length == 1)
        {
            var watchers = app.DataSource.GetWatchers(app.ServiceProvider);

            Console.WriteLine($"Available Resources:");
            foreach (var watcher in watchers) {
                Console.WriteLine($"  {watcher.Controller.ResourceType.Name}");
            }

            return Task.CompletedTask;
        }

        Console.WriteLine(KubernetesYaml.Serialize(new V1Project().Initialize()));

        return Task.CompletedTask;
    }
}
