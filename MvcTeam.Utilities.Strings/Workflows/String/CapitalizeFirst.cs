using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_CapitalizeFirst : WorkFlowActivityBase
{
    public String_CapitalizeFirst() : base(typeof(String_CapitalizeFirst)) { }

    [RequiredArgument]
    [Input("String To Capitalize")]
    public InArgument<string> StringToCapitalize { get; set; }

    [Output("Capitalized String")]
    public OutArgument<string> CapitalizedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string text = StringToCapitalize.Get(context) ?? "";

        //Only the very first letter changes; use To Title Case to capitalize every word
        string capitalized = text.Length == 0 ? "" : char.ToUpperInvariant(text[0]) + text.Substring(1);

        CapitalizedString.Set(context, capitalized);
    }
}