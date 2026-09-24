using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;

namespace MvcTeam.Utilities.Workflows
{
    //Reads up to two fields from the first record that matches one or two "field = value" filters.
    //Based on QueryValues from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko)
    public class QueryValues : CodeActivity
    {
        [RequiredArgument]
        [Input("EntityName")]
        [Default("")]
        public InArgument<string> EntityName { get; set; }

        [RequiredArgument]
        [Input("Attribute1")]
        [ReferenceTarget("")]
        public InArgument<string> Attribute1 { get; set; }

        [Input("Attribute2")]
        [ReferenceTarget("")]
        public InArgument<string> Attribute2 { get; set; }

        [RequiredArgument]
        [Input("FilterAttribute1")]
        [ReferenceTarget("")]
        public InArgument<string> FilterAttribute1 { get; set; }

        //Empty means "FilterAttribute1 is empty"
        [Input("ValueAttribute1")]
        [ReferenceTarget("")]
        public InArgument<string> ValueAttribute1 { get; set; }

        [Input("FilterAttribute2")]
        [ReferenceTarget("")]
        public InArgument<string> FilterAttribute2 { get; set; }

        [Input("ValueAttribute2")]
        [ReferenceTarget("")]
        public InArgument<string> ValueAttribute2 { get; set; }

        [Output("ResultValue1")]
        public OutArgument<string> ResultValue1 { get; set; }

        [Output("ResultValue2")]
        public OutArgument<string> ResultValue2 { get; set; }

        [Output("Record Found")]
        public OutArgument<bool> RecordFound { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var entityName = Required(EntityName.Get(context), "EntityName");
            var attribute1 = Required(Attribute1.Get(context), "Attribute1");
            var filterAttribute1 = Required(FilterAttribute1.Get(context), "FilterAttribute1");
            var attribute2 = Attribute2.Get(context)?.Trim();
            var filterAttribute2 = FilterAttribute2.Get(context)?.Trim();

            var columns = new List<string> { attribute1 };
            if (!string.IsNullOrEmpty(attribute2)) columns.Add(attribute2);

            var filters = new Dictionary<string, string> { [filterAttribute1] = ValueAttribute1.Get(context) };
            if (!string.IsNullOrEmpty(filterAttribute2)) filters[filterAttribute2] = ValueAttribute2.Get(context);

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //Runs as the workflow's user, so it only finds what that user can read
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            Entity record;
            try
            {
                record = new CrmService(service, tracingService).FindFirst(entityName, columns, filters);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Query Values failed: {ex.Message}", ex);
            }

            tracingService.Trace("Query on {0}: {1}", entityName, record == null ? "no record found" : "found " + record.Id);
            RecordFound.Set(context, record != null);
            ResultValue1.Set(context, ToText(record, attribute1));
            ResultValue2.Set(context, string.IsNullOrEmpty(attribute2) ? "" : ToText(record, attribute2));
        }

        //Lookups give the record id, option sets their number, dates ISO in UTC, numbers with '.' decimals
        private static string ToText(Entity record, string attribute)
        {
            if (record == null || !record.Contains(attribute)) return "";
            var value = record[attribute] is AliasedValue aliased ? aliased.Value : record[attribute];

            switch (value)
            {
                case null: return "";
                case EntityReference reference: return reference.Id.ToString();
                case OptionSetValue option: return option.Value.ToString(CultureInfo.InvariantCulture);
                case Money money: return money.Value.ToString(CultureInfo.InvariantCulture);
                case bool flag: return flag ? "true" : "false";
                case DateTime date:
                    var utc = date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : DateTime.SpecifyKind(date, DateTimeKind.Utc);
                    return utc.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
                case IFormattable formattable: return formattable.ToString(null, CultureInfo.InvariantCulture);
                default: return value.ToString();
            }
        }

        private static string Required(string value, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidPluginExecutionException($"{label} is required.");
            return value.Trim();
        }
    }
}
