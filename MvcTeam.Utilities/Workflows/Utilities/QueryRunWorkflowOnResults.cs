using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    //Starts a workflow on every record a view or FetchXml returns.
    //Based on QueryRunWorkflowOnResults from Kaskela.WorkflowElements.
    public class QueryRunWorkflowOnResults : CodeActivity
    {
        [Input("Pick a System View to Use")]
        [ReferenceTarget("savedquery")]
        public InArgument<EntityReference> SavedQuery { get; set; }

        [Input("or pick a Personal View to Use")]
        [ReferenceTarget("userquery")]
        public InArgument<EntityReference> UserQuery { get; set; }

        [Input("or enter FetchXML to Use")]
        public InArgument<string> FetchXml { get; set; }

        [RequiredArgument]
        [Input("Workflow to Run")]
        [ReferenceTarget("workflow")]
        public InArgument<EntityReference> Workflow { get; set; }

        [Output("Number of Workflows Started")]
        public OutArgument<int> NumberOfWorkflowsStarted { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var workflow = Workflow.Get(context);
            if (workflow == null) throw new InvalidPluginExecutionException("Workflow to Run is required.");

            var savedQuery = SavedQuery.Get(context);
            var userQuery = UserQuery.Get(context);
            var fetchXml = FetchXml.Get(context);
            if (savedQuery == null && userQuery == null && string.IsNullOrWhiteSpace(fetchXml))
                throw new InvalidPluginExecutionException("You need to pick a System View, a Personal View, or specify FetchXML for the query.");

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //Runs as the workflow's user, so it only finds and starts workflows on what that user may
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var records = QueryResultTable.LoadRecordIds(service, savedQuery, userQuery, fetchXml,
                workflowContext.PrimaryEntityName, workflowContext.PrimaryEntityId);
            tracingService.Trace("Query returned {0} {1} record(s)", records.Ids.Count, records.EntityName);

            var started = new CrmService(service, tracingService).RunWorkflowOnRecords(workflow.Id, records);

            tracingService.Trace("Started workflow {0} on {1} record(s)", workflow.Id, started);
            NumberOfWorkflowsStarted.Set(context, started);
        }
    }
}
