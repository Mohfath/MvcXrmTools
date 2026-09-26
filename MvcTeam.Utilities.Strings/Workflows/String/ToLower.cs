using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_ToLower : WorkFlowActivityBase
{
    public String_ToLower() : base(typeof(String_ToLower)) { }

    [RequiredArgument]
    [Input("String To Lower")]
    public InArgument<string> StringToLower { get; set; }

    [Output("Lowered String")]
    public OutArgument<string> LoweredString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToLower = StringToLower.Get(context);

        string loweredString = stringToLower.ToLower();

        LoweredString.Set(context, loweredString);
    }
}
