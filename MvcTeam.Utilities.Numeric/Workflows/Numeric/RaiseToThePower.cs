using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_RaiseToThePower : WorkFlowActivityBase
{
    public Numeric_RaiseToThePower() : base(typeof(Numeric_RaiseToThePower)) { }

    [RequiredArgument]
    [Input("Number")]
    public InArgument<double> Number { get; set; }

    [RequiredArgument]
    [Input("Power Number")]
    [Default("2")]
    public InArgument<double> PowerNumber { get; set; }

    [RequiredArgument]
    [Input("Round Decimal Places")]
    [Default("-1")]
    public InArgument<int> RoundDecimalPlaces { get; set; }

    [Output("Result")]
    public OutArgument<double> Result { get; set; }
    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        var number = Number.Get(context);
        var powerNumber = PowerNumber.Get(context);
        var roundDecimalPlaces = RoundDecimalPlaces.Get(context);

        var result = Math.Pow(number, powerNumber);

        if (roundDecimalPlaces != -1)
            result = Math.Round(result, roundDecimalPlaces, MidpointRounding.AwayFromZero);

        Result.Set(context, result);
    }
}
