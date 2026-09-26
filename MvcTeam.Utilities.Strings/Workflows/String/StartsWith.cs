using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_StartsWith : WorkFlowActivityBase
{
    public String_StartsWith() : base(typeof(String_StartsWith)) { }

    [RequiredArgument]
    [Input("String To Search")]
    public InArgument<string> StringToSearch { get; set; }

    [RequiredArgument]
    [Input("Search For")]
    public InArgument<string> SearchFor { get; set; }

    [RequiredArgument]
    [Input("Case Sensitive")]
    [Default("false")]
    public InArgument<bool> CaseSensitive { get; set; }

    [Output("Starts With String")]
    public OutArgument<bool> StartsWithString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToSearch = StringToSearch.Get(context);
        string searchFor = SearchFor.Get(context);
        bool caseSensitive = CaseSensitive.Get(context);

        bool startsWithString = stringToSearch.StartsWith(searchFor,
            (caseSensitive) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);

        StartsWithString.Set(context, startsWithString);
    }
}
