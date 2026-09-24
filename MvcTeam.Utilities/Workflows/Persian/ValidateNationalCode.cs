using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    public class ValidateNationalCode : CodeActivity
    {
        [Input("National Code to Validate")]
        public InArgument<string> NationalCode { get; set; }

        [Output("Is Valid")]
        public OutArgument<bool> IsValid { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ITracingService tracingService = context.GetExtension<ITracingService>();

            var isValid = NationalCodeValidator.IsValid(NationalCode.Get(context));

            //The code itself is personal data, so only the result is traced
            tracingService.Trace("National code valid: {0}", isValid);
            IsValid.Set(context, isValid);
        }
    }
}
