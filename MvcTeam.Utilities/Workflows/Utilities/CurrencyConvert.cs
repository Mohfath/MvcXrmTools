using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    //Converts an amount between currencies using the exchange rates maintained in CRM
    //(Settings > Business Management > Currencies), so no internet access is needed.
    //Based on CurrencyConvert from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko), which used an online service that no longer exists.
    public class CurrencyConvert : CodeActivity
    {
        [RequiredArgument]
        [Input("Amount")]
        [Default("0")]
        public InArgument<decimal> Amount { get; set; }

        //ISO code, e.g. IRR, USD, EUR
        [RequiredArgument]
        [Input("From Currency")]
        [Default("")]
        public InArgument<string> FromCurrency { get; set; }

        [RequiredArgument]
        [Input("To Currency")]
        [Default("")]
        public InArgument<string> ToCurrency { get; set; }

        [Output("Result")]
        public OutArgument<decimal> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var from = FromCurrency.Get(context);
            if (string.IsNullOrWhiteSpace(from)) throw new InvalidPluginExecutionException("From Currency is required.");
            var to = ToCurrency.Get(context);
            if (string.IsNullOrWhiteSpace(to)) throw new InvalidPluginExecutionException("To Currency is required.");
            var amount = Amount.Get(context);

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var result = new CrmService(service, tracingService).ConvertCurrency(amount, from, to);

            tracingService.Trace("Converted {0} {1} to {2} {3}", amount, from, result, to);
            Result.Set(context, result);
        }
    }
}
