using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

//The user whose action started the workflow (not the workflow's owner, which background workflows run as).
//Based on GetInitiatingUser from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko)
public class Security_GetInitiatingUser : CodeActivity
{
    [Output("Initiating User")]
    [ReferenceTarget("systemuser")]
    public OutArgument<EntityReference> InitiatingUser { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

        context.GetExtension<ITracingService>().Trace("Initiating user: {0}", workflowContext.InitiatingUserId);
        InitiatingUser.Set(context, new EntityReference("systemuser", workflowContext.InitiatingUserId));
    }
}
