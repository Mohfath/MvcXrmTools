using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MvcTeam.Utilities.Workflows
{
    public class RandomNumber : WorkFlowActivityBase
    {
        public RandomNumber() : base(typeof(RandomNumber)) { }

        [RequiredArgument]
        [Input("Max Value")]
        public InArgument<int> MaxValue { get; set; }

        [Output("Generated Number")]
        public OutArgument<int> GeneratedNumber { get; set; }

        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            int maxValue = MaxValue.Get(context);

            if (maxValue < 1)
                maxValue = 1;

            System.Random random = new System.Random();
            int generatedNumber = random.Next(maxValue);

            GeneratedNumber.Set(context, generatedNumber);
        }
    }
}