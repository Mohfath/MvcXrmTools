using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.PersianCalendar.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.PersianCalendar.Workflows
{


    public class ConvertNumberToPersianString : CodeActivity
    {
        //Time: 1 hrs
        
        [Input("Decimal Input")]
        [ReferenceTarget("decimal")]
        public InArgument<decimal> InputDecimal { get; set; }


        [Output("OutputText")]
        public OutArgument<string> Output { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            object entityReference = null;
            string output = "";

             entityReference = InputDecimal.Get(context); 





            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            // Use the context service to create an instance of IOrganizationService.             
            IOrganizationService _service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
            ITracingService tracingService = context.GetExtension<ITracingService>();


            tracingService.Trace("شیئ داده ما دریافت شد");

            //بارگزاری موتور برنامه
            if (entityReference != null)
            {


                try
                {
                    output = AppConvertNumberToLetters.ToLetters(entityReference);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }

            }
            Output.Set(context, output);

        }

    }
}
