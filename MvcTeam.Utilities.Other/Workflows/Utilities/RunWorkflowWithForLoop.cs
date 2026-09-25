using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class Utilities_RunWorkflowWithForLoop : CodeActivity
{
    private const int MaxRepeatCount = 100;

    [RequiredArgument]
    [Input("Record ID")]
    [ReferenceTarget("")]
    public InArgument<String> RecordID { get; set; }

    [RequiredArgument]
    [Input("# of Repeats")]
    public InArgument<int> RepeatCount { get; set; }

    [Input("Process")]
    [ReferenceTarget("workflow")]
    public InArgument<EntityReference> Process { get; set; }



    protected override void Execute(CodeActivityContext context)
    {
        IExecutionContext executionContext = context.GetExtension<IExecutionContext>();
        IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
        IOrganizationService service = serviceFactory.CreateOrganizationService(executionContext.UserId);
        ITracingService tracingService = context.GetExtension<ITracingService>();

        String _RecordID = this.RecordID.Get(context);
        int _repeatCount = this.RepeatCount.Get(context);

        EntityReference process = this.Process.Get(context);

        if (process == null)
            throw new InvalidPluginExecutionException("Process is required.");
        Guid recordId;
        if (!Guid.TryParse(_RecordID, out recordId))
            throw new InvalidPluginExecutionException($"Record ID '{_RecordID}' is not a valid GUID.");
        if (_repeatCount < 1 || _repeatCount > MaxRepeatCount)
            throw new InvalidPluginExecutionException($"# of Repeats must be between 1 and {MaxRepeatCount}; got {_repeatCount}.");


        #region "SetProcess Execution"

        ExecuteWorkflowRequest wfRequest = new ExecuteWorkflowRequest();
        wfRequest.EntityId = recordId;
        wfRequest.WorkflowId = process.Id;
        for (int i = 0; i < _repeatCount; i++)
        {
            service.Execute(wfRequest);
        }

        #endregion

    }
}
