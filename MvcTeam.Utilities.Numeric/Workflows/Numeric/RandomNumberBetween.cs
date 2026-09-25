using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_RandomNumberBetween : WorkFlowActivityBase
{
    public Numeric_RandomNumberBetween() : base(typeof(Numeric_RandomNumberBetween)) { }

    [RequiredArgument]
    [Input("Min Value")]
    public InArgument<int> MinValue { get; set; }

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

        int minValue = MinValue.Get(context);
        int maxValue = MaxValue.Get(context);

        if (minValue < 1)
            minValue = 0;

        if (maxValue < 1)
            maxValue = 1;

        if (maxValue < minValue)
            throw new InvalidPluginExecutionException("Max Value must be greater than Min Value.");

        if (maxValue == minValue)
        {
            GeneratedNumber.Set(context, maxValue);
            return;
        }

        System.Random random = new System.Random();
        int generatedNumber = random.Next(minValue, maxValue);

        GeneratedNumber.Set(context, generatedNumber);
    }
}
