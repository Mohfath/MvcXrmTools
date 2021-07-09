using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.Workflows
{
    public class UpdateFieldDynamically : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IExecutionContext executionContext = context.GetExtension<IExecutionContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(executionContext.UserId);
            ITracingService tracingService = context.GetExtension<ITracingService>();
            if (tracingService == null)
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");

            tracingService.Trace($"DynamicFieldName = {DynamicFieldName.Get(context)}");
            tracingService.Trace($"DynamicFieldGuid = {FieldGuid.Get(context)}");
            tracingService.Trace($"DynamicFieldLogicalName = {LookupFieldLogicalName.Get(context)}");
            tracingService.Trace($"DynamicFieldLookup = {DynamicFieldLookup.Get(context)}");
            tracingService.Trace($"DynamicFieldType = {DynamicFieldType.Get(context)}");
            tracingService.Trace($"DynamicFieldValue = {FieldValue.Get(context)}");


            if (this.DynamicFieldName.Get(context) != null && this.DynamicFieldType.Get(context) != null)
            {
                //اگر برای ما آدرس داینامیک رکورد را فرستاده اند
                var eReference = new EntityReference();
                var baseReference = new EntityReference();

                if (DynamicFieldLookup.Get(context) != null && DynamicFieldLookup.Get(context) != "")
                {
                    var lookup = new DynamicUrlParser(DynamicFieldLookup.Get(context));
                    baseReference = new EntityReference(lookup.GetEntityLogicalName(service), lookup.Id);
                }

                //اگر برای ما شناسه و نوع رکورد را فرستاده اند
                if (LookupFieldLogicalName.Get(context) != null && LookupFieldLogicalName.Get(context) != "")
                {
                    eReference = new EntityReference(LookupFieldLogicalName.Get(context), Guid.Parse(FieldGuid.Get(context)));
                }

                var type = DynamicFieldType.Get(context);
                var fieldName = DynamicFieldName.Get(context);
                var input = FieldValue.Get(context);
                var baseRecord = new Entity(baseReference.LogicalName, baseReference.Id);

                object value = null;
                switch (type)
                {
                    case "string":
                        value = input;
                        break;
                    case "int":
                        value = int.Parse(input);
                        break;
                    case "money":
                        value = new Money(Decimal.Parse(input));
                        break;
                    case "optionset":
                        value = new OptionSetValue(int.Parse(input));
                        break;
                    case "decimal":
                        value = decimal.Parse(input);
                        break;
                    case "lookup":
                        value = eReference;
                        break;

                    default:
                        break;
                }

                baseRecord[fieldName] = value;
                service.Update(baseRecord);
            }
        }


        //Base Record is Get from this
        [Input("This Record DynamicUrl")]
        public InArgument<string> DynamicFieldLookup { get; set; }

        [Input("Field To Update")]
        public InArgument<string> DynamicFieldName { get; set; }

        [Input("int,decimal,money,string,lookup")]
        public InArgument<string> DynamicFieldType { get; set; }

        [Input("Lookup Target Entity Logical Name")]
        public InArgument<string> LookupFieldLogicalName { get; set; }

        [Input("New Value (Empty if using Lookup)")]
        public InArgument<string> FieldValue { get; set; }

        [Input("Lookup Target Guid")]
        public InArgument<string> FieldGuid { get; set; }


    }
}
