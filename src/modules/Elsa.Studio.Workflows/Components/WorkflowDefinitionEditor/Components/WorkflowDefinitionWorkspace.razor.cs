using Elsa.Api.Client.Resources.WorkflowDefinitions.Models;
using Elsa.Api.Client.Shared.Models;
using Elsa.Studio.Workflows.Domain.Contracts;
using Elsa.Studio.Workflows.Models;
using Elsa.Studio.Workflows.UI.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Elsa.Studio.Workflows.Components.WorkflowDefinitionEditor.Components;

/// <summary>
/// A workspace for editing a workflow definition.
/// </summary>
public partial class WorkflowDefinitionWorkspace : IWorkspace
{
    private MudDynamicTabs _dynamicTabs = default!;

    /// <summary>
    /// Gets or sets the workflow definition to edit.
    /// </summary>
    [Parameter]
    public WorkflowDefinition WorkflowDefinition { get; set; } = default!;

    /// <summary>
    /// Gets or sets a specific version of the workflow definition to view.
    /// </summary>
    [Parameter]
    public WorkflowDefinition SelectedWorkflowDefinition { get; set; } = default!;

    /// <summary>An event that is invoked when a workflow definition has been executed.</summary>
    /// <remarks>The ID of the workflow instance is provided as the value to the event callback.</remarks>
    [Parameter]
    public EventCallback<string> WorkflowDefinitionExecuted { get; set; }

    /// <summary>
    /// Gets or sets the event that occurs when the workflow definition version is updated.
    /// </summary>
    [Parameter]
    public EventCallback<WorkflowDefinition> WorkflowDefinitionVersionSelected { get; set; }

    /// <summary>
    /// An event that is invoked when the workflow definition is updated.
    /// </summary>
    public event Func<Task>? WorkflowDefinitionUpdated;

    /// <inheritdoc />
    public bool IsReadOnly => SelectedWorkflowDefinition?.IsLatest == false;

    /// <inheritdoc />
    public ApiOperationPermissions Permissions { get; set; }
    
    [Inject] private IWorkflowDefinitionService WorkflowDefinitionService { get; set; } = default!;

    private WorkflowEditor WorkflowEditor { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (SelectedWorkflowDefinition == null!)
        {
            SelectedWorkflowDefinition = WorkflowDefinition;
            Permissions = new ApiOperationPermissions(SelectedWorkflowDefinition?.Links ?? []);
        }
    }

    /// <summary>
    /// Displays the specified workflow definition version.
    /// </summary>
    public void DisplayWorkflowDefinitionVersion(WorkflowDefinition workflowDefinition)
    {
        SelectedWorkflowDefinition = workflowDefinition;
        Permissions = new ApiOperationPermissions(SelectedWorkflowDefinition?.Links ?? []);

        if (WorkflowDefinitionVersionSelected.HasDelegate)
            WorkflowDefinitionVersionSelected.InvokeAsync(SelectedWorkflowDefinition);

        StateHasChanged();
    }

    /// <summary>
    /// Gets the currently selected workflow definition version.
    /// </summary>
    public WorkflowDefinition? GetSelectedWorkflowDefinitionVersion() => SelectedWorkflowDefinition;

    /// <summary>
    /// Refreshes the active workflow definition.
    /// </summary>
    public async Task RefreshActiveWorkflowAsync()
    {
        var definitionId = WorkflowDefinition.DefinitionId;
        var definition = await WorkflowDefinitionService.FindByDefinitionIdAsync(definitionId, VersionOptions.Latest);
        SelectedWorkflowDefinition = definition!;
        Permissions = new ApiOperationPermissions(SelectedWorkflowDefinition?.Links ?? []);
        StateHasChanged();
    }

    private async Task OnWorkflowDefinitionPropsUpdated()
    {
        await WorkflowEditor.NotifyWorkflowChangedAsync();
    }

    private async Task OnWorkflowDefinitionUpdated()
    {
        SelectedWorkflowDefinition = WorkflowEditor.WorkflowDefinition!;
        Permissions = new ApiOperationPermissions(SelectedWorkflowDefinition?.Links ?? []);
        StateHasChanged();

        if (WorkflowDefinitionUpdated != null)
            await WorkflowDefinitionUpdated();
    }
}