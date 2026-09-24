using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    public class RecalculateAllRollups : CodeActivity
    {
        //A "Record Url (Dynamic)" value, e.g. from the record's own dynamic URL field
        [Input("Record Reference")]
        [RequiredArgument]
        public InArgument<string> Record { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var recordUrl = Record.Get(context);
            if (string.IsNullOrWhiteSpace(recordUrl)) throw new InvalidPluginExecutionException("Record Reference is required.");

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //System service, so every rollup can be recalculated whatever the running user can read
            IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

            var crmService = new CrmService(systemService, tracingService);
            var target = crmService.GetRecordFromUrl(recordUrl);
            var fields = crmService.RecalculateAllRollups(target);

            tracingService.Trace("Recalculated {0} rollup field(s) on {1} {2}", fields.Count, target.LogicalName, target.Id);
        }
    }
}
