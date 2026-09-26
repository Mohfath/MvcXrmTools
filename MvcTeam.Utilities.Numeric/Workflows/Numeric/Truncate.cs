using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_Truncate : WorkFlowActivityBase
{
    public Numeric_Truncate() : base(typeof(Numeric_Truncate)) { }

    [RequiredArgument]
    [Input("Number To Truncate")]
    public InArgument<decimal> NumberToTruncate { get; set; }

    [RequiredArgument]
    [Input("Decimal Places")]
    public InArgument<int> DecimalPlaces { get; set; }

    [Output("Truncated Number")]
    public OutArgument<decimal> TruncatedNumber { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        decimal numberToTruncate = NumberToTruncate.Get(context);
        int decimalPlaces = DecimalPlaces.Get(context);

        if (decimalPlaces < 0)
            decimalPlaces = 0;

        decimal step = (decimal)Math.Pow(10, decimalPlaces);
        int temp = (int)Math.Truncate(step * numberToTruncate);

        decimal truncatedNumber = temp / step;

        TruncatedNumber.Set(context, truncatedNumber);
    }
}
