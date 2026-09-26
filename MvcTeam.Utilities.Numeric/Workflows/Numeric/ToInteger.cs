using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Services;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Numeric_ToInteger : WorkFlowActivityBase
{
    public Numeric_ToInteger() : base(typeof(Numeric_ToInteger)) { }

    [RequiredArgument]
    [Input("Text To Convert")]
    public InArgument<string> TextToConvert { get; set; }

    [Output("Converted Number")]
    public OutArgument<int> ConvertedNumber { get; set; }

    [Output("Is Valid Integer")]
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

        bool isNumber = NumberText.TryParseInteger(textToConvert, out var convertedNumber);

        if (isNumber)
        {
            ConvertedNumber.Set(context, convertedNumber);
            IsValid.Set(context, true);
        }
        else
            IsValid.Set(context, false);
    }
}
