using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class String_Length : WorkFlowActivityBase
{
    public String_Length() : base(typeof(String_Length)) { }

    [RequiredArgument]
    [Input("String To Measure")]
    public InArgument<string> StringToMeasure { get; set; }

    [Output("String Length")]
    public OutArgument<int> StringLength { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToMeasure = StringToMeasure.Get(context);
        if (string.IsNullOrEmpty(stringToMeasure))
        {
            StringLength.Set(context, 0);
            return;
        }

        int stringLength = stringToMeasure.Length;

        StringLength.Set(context, stringLength);
    }
}
