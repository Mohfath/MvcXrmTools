using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

public class Utilities_CloneRecord : CodeActivity
{
    [RequiredArgument]
    [Input("Cloning Record URL")]
    [ReferenceTarget("")]
    public InArgument<string> CloningRecordURL { get; set; }

    [Input("Prefix")]
    [Default("")]
    public InArgument<string> Prefix { get; set; }

    //Logical names separated by ';' or ','
    [Input("Fields to Ignore")]
    [Default("")]
    public InArgument<string> FieldsToIgnore { get; set; }

    [Output("Cloned Guid")]
    public OutArgument<string> ClonedGuid { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
        var recordUrl = CloningRecordURL.Get(context);
        if (string.IsNullOrWhiteSpace(recordUrl)) throw new InvalidPluginExecutionException("Cloning Record URL is required.");

        ITracingService tracingService = context.GetExtension<ITracingService>();
        IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

        //Runs as the workflow's user, so users can only clone what they could create themselves
        IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

        var source = new CrmService(service, tracingService).GetRecordFromUrl(recordUrl);
        var copyId = new RecordCloner(service, tracingService).Clone(source, FieldsToIgnore.Get(context), Prefix.Get(context));

        tracingService.Trace("Cloned {0} {1} into {2}", source.LogicalName, source.Id, copyId);
        ClonedGuid.Set(context, copyId.ToString());
    }
}
