using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text.RegularExpressions;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_RegexMatch : WorkFlowActivityBase
{
    public String_RegexMatch() : base(typeof(String_RegexMatch)) { }

    [RequiredArgument]
    [Input("String To Search")]
    public InArgument<string> StringToSearch { get; set; }

    [RequiredArgument]
    [Input("Pattern")]
    public InArgument<string> Pattern { get; set; }

    [Output("Contains Pattern")]
    public OutArgument<bool> ContainsPattern { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToSearch = StringToSearch.Get(context);
        string pattern = Pattern.Get(context);

        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        Match match = regex.Match(stringToSearch);
        bool containsPattern = match.Success;

        ContainsPattern.Set(context, containsPattern);
    }
}
