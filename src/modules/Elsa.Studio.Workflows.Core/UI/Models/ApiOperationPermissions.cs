using Elsa.Api.Client.Shared.Models;

namespace Elsa.Studio.Workflows.Models;

public class ApiOperationPermissions
{
    private Link[] links;

    public ApiOperationPermissions()
    {
        links = [];
    }
    
    public ApiOperationPermissions(Link[] links)
    {
        this.links = links;
    }

    public bool CanExecuteOperation(string operation)
        => links.Count(l => l.Rel == operation) > 0;
}