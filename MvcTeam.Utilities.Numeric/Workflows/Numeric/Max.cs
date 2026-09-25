using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_Max : WorkFlowActivityBase
{
    public Numeric_Max() : base(typeof(Numeric_Max)) { }

    [RequiredArgument]
    [Input("Number 1")]
    public InArgument<decimal> Number1 { get; set; }

    [RequiredArgument]
    [Input("Number 2")]
    public InArgument<decimal> Number2 { get; set; }

    [Output("Max Value")]
    public OutArgument<decimal> MaxValue { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        decimal number1 = Number1.Get(context);
        decimal number2 = Number2.Get(context);

        decimal maxValue = number1 >= number2 ? number1 : number2;

        MaxValue.Set(context, maxValue);
    }
}
