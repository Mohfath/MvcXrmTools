using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_Average : WorkFlowActivityBase
{
    public Numeric_Average() : base(typeof(Numeric_Average)) { }

    [RequiredArgument]
    [Input("Number 1")]
    public InArgument<decimal> Number1 { get; set; }

    [RequiredArgument]
    [Input("Number 2")]
    public InArgument<decimal> Number2 { get; set; }

    [RequiredArgument]
    [Input("Round Decimal Places")]
    [Default("-1")]
    public InArgument<int> RoundDecimalPlaces { get; set; }

    [Output("Average Value")]
    public OutArgument<decimal> AverageValue { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        decimal number1 = Number1.Get(context);
        decimal number2 = Number2.Get(context);
        int roundDecimalPlaces = RoundDecimalPlaces.Get(context);

        decimal averageValue = (number1 + number2) / 2;

        if (roundDecimalPlaces != -1)
            averageValue = Math.Round(averageValue, roundDecimalPlaces);

        AverageValue.Set(context, averageValue);
    }
}
