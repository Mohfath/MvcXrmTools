using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Services;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_ToDecimal : WorkFlowActivityBase
{
    public Numeric_ToDecimal() : base(typeof(Numeric_ToDecimal)) { }

    [RequiredArgument]
    [Input("Text To Convert")]
    public InArgument<string> TextToConvert { get; set; }

    [Output("Converted Number")]
    public OutArgument<decimal> ConvertedNumber { get; set; }

    [Output("Is Valid Decimal")]
    public OutArgument<bool> IsValid { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string textToConvert = TextToConvert.Get(context);

        if (string.IsNullOrEmpty(textToConvert))
        {
            IsValid.Set(context, false);
            return;
        }

        bool isNumber = NumberText.TryParseDecimal(textToConvert, out var convertedNumber);

        if (isNumber)
        {
            ConvertedNumber.Set(context, convertedNumber);
            IsValid.Set(context, true);
        }
        else
            IsValid.Set(context, false);
    }
}
