using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    //Clones every child of the source record (through a one-to-many relationship)
    //and attaches the copies to the target record.
    public class CloneChildren : CodeActivity
    {
        [RequiredArgument]
        [Input("Source Record URL")]
        [ReferenceTarget("")]
        public InArgument<string> SourceRecordUrl { get; set; }

        [RequiredArgument]
        [Input("Target Record URL")]
        [ReferenceTarget("")]
        public InArgument<string> TargetRecordUrl { get; set; }

        [RequiredArgument]
        [Input("Relationship Name")]
        [ReferenceTarget("")]
        public InArgument<string> RelationshipName { get; set; }

        [RequiredArgument]
        [Input("New Parent Field Name")]
        [ReferenceTarget("")]
        public InArgument<string> NewParentFieldNameToUpdate { get; set; }

        //Optional: cleared on the copies when the new parent uses a different lookup field
        [Input("Old Parent Field Name")]
        [ReferenceTarget("")]
        public InArgument<string> OldParentFieldNameToUpdate { get; set; }

        [Input("Prefix")]
        [Default("")]
        public InArgument<string> Prefix { get; set; }

        //Logical names separated by ';' or ','
        [Input("Fields to Ignore")]
        [Default("")]
        public InArgument<string> FieldsToIgnore { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var sourceUrl = Required(SourceRecordUrl.Get(context), "Source Record URL");
            var targetUrl = Required(TargetRecordUrl.Get(context), "Target Record URL");
            var relationshipName = Required(RelationshipName.Get(context), "Relationship Name");
            var newParentField = Required(NewParentFieldNameToUpdate.Get(context), "New Parent Field Name");

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //Runs as the workflow's user, so users can only clone what they could create themselves
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var crmService = new CrmService(service, tracingService);
            var source = crmService.GetRecordFromUrl(sourceUrl);
            var target = crmService.GetRecordFromUrl(targetUrl);

            var count = new RecordCloner(service, tracingService).CloneChildren(relationshipName, source, newParentField, target,
                OldParentFieldNameToUpdate.Get(context), FieldsToIgnore.Get(context), Prefix.Get(context));

            tracingService.Trace("Cloned {0} child record(s) of {1} {2} to {3} {4}", count, source.LogicalName, source.Id, target.LogicalName, target.Id);
        }

        private static string Required(string value, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidPluginExecutionException($"{label} is required.");
            return value;
        }
    }
}
