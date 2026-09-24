using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    public class GetRecordId : CodeActivity
    {
        //A "Record Url (Dynamic)" value, e.g. from the record's own dynamic URL field
        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        [Output("Id")]
        public OutArgument<string> RecordId { get; set; }

        [Output("Entity Type Name")]
        public OutArgument<string> EntityTypeName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var recordUrl = Record.Get(context);
            if (string.IsNullOrWhiteSpace(recordUrl)) throw new InvalidPluginExecutionException("Record Reference is required.");

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //System service: old-style URLs need a metadata lookup, which shouldn't depend on the running user
            IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

            var target = new CrmService(systemService, tracingService).GetRecordFromUrl(recordUrl);

            tracingService.Trace("Record: {0} {1}", target.LogicalName, target.Id);
            RecordId.Set(context, target.Id.ToString());
            EntityTypeName.Set(context, target.LogicalName);
        }
    }
}
