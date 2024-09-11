using k8s.Models;
using K8sOperator.NET;
using SonarCloud.NET;

namespace Sonarcloud.Projects;

public class ProjectsController(ISonarCloudApiClient client, ILogger<ProjectsController> logger) : Controller<V1Project>
{
    public ISonarCloudApiClient Client { get; } = client;
    public ILogger<ProjectsController> Logger { get; } = logger;

    public override async Task AddOrModifyAsync(V1Project resource, CancellationToken cancellationToken)
    {
        var projectName = string.IsNullOrEmpty(resource.Spec.DisplayName) ?
                $"{resource.Namespace()}: {resource.Name()}" :
                resource.Spec.DisplayName;

        switch (resource.Status?.State)
        {
            case ProjectState.Pending:
                Logger.LogInformation("Creating project {Name}.", projectName);

                var results = await Client.Projects.Create(new()
                {
                    Name = projectName,
                    Organization = resource.Spec.Organization,
                    Project = resource.Spec.ProjectKey,
                }, cancellationToken);

                resource.SetLabel("state", ProjectState.AwaitTemplate.ToString());
                resource.Status = new()
                {
                    Name = projectName,
                    Key = results.Project.Key,
                    Visibility = results.Project.Visibility ?? string.Empty,
                    State = ProjectState.AwaitTemplate
                };

                break;
            case ProjectState.AwaitTemplate:

                await Client.Permissions.ApplyTemplate(new()
                {
                    Organization = resource.Spec.Organization,
                    ProjectKey = resource.Spec.ProjectKey,
                    TemplateName = resource.Spec.TemplateName
                }, cancellationToken);

                resource.SetLabel("state", ProjectState.Created.ToString());
                resource.Status.State = ProjectState.Created;

                break;

            case ProjectState.Created:
                if (resource.Status.Key != resource.Spec.ProjectKey)
                {
                    await Client.Projects.UpdateKey(new() { From = resource.Status.Key, To = resource.Spec.ProjectKey }, cancellationToken);
                    resource.Status.Key = resource.Spec.ProjectKey;
                }
                break;
            default:
                
                break;
        }
    }

    public override async Task DeleteAsync(V1Project resource, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(resource.Status.Key))
        {
            return;
        }

        await Client.Projects.Delete(new()
        {
            Project = resource.Status.Key
        }, cancellationToken);
    }
}
