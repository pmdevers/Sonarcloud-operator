using k8s.Models;
using K8sOperator.NET.Models;

namespace Sonarcloud.Projects;

[KubernetesEntity(ApiVersion = "v1", Group = "sonarcloud.io", Kind = "Project", PluralName = "projects" )]
public class V1Project : CustomResource<V1Project.V1ProjectSpec, V1Project.V1ProjectStatus>
{
    public class V1ProjectSpec
    {
        public string? DisplayName { get;set; } = null;
        public string Organization { get; set; } = string.Empty;
        public string ProjectKey { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
    }

    public class V1ProjectStatus
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public ProjectState State { get; set; } = ProjectState.Pending;
    }
}

public enum ProjectState
{
    Pending = 1,
    AwaitTemplate = 2,
    Created = 3,
    
}
