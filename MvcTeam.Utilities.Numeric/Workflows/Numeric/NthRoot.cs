using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_NthRoot : WorkFlowActivityBase
{
    public Numeric_NthRoot() : base(typeof(Numeric_NthRoot)) { }

    [RequiredArgument]
    [Input("Number")]
    [Default("1")]
    public InArgument<double> Number { get; set; }

    [RequiredArgument]
    [Input("Root Number")]
    [Default("2")]
    public InArgument<double> RootNumber { get; set; }

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
        var rootNumber = RootNumber.Get(context);
        var roundDecimalPlaces = RoundDecimalPlaces.Get(context);

        var result = Math.Pow(number, 1 / rootNumber);

        if (double.IsNaN(result))
            throw new InvalidPluginExecutionException("Number under the radical must be positive.");

        if (roundDecimalPlaces != -1)
            result = Math.Round(result, roundDecimalPlaces);

        Result.Set(context, result);
    }
}
