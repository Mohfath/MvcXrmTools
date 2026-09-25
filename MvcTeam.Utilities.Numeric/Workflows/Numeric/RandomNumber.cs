using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Services;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_RandomNumber : WorkFlowActivityBase
{
    public Numeric_RandomNumber() : base(typeof(Numeric_RandomNumber)) { }

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

        int generatedNumber = SecureRandom.Next(0, maxValue);

        GeneratedNumber.Set(context, generatedNumber);
    }
}
