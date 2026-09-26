using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_ToUpper : WorkFlowActivityBase
{
    public String_ToUpper() : base(typeof(String_ToUpper)) { }

    [RequiredArgument]
    [Input("String To Upper")]
    public InArgument<string> StringToUpper { get; set; }

    [Output("Uppered String")]
    public OutArgument<string> UpperedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToUpper = StringToUpper.Get(context);
        string upperedString = stringToUpper.ToUpper();

        UpperedString.Set(context, upperedString);
    }
}
