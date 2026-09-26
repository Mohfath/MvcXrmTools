using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_Replace : WorkFlowActivityBase
{
    public String_Replace() : base(typeof(String_Replace)) { }

    [RequiredArgument]
    [Input("String To Search")]
    public InArgument<string> StringToSearch { get; set; }

    [RequiredArgument]
    [Input("Value To Replace")]
    public InArgument<string> ValueToReplace { get; set; }

    [Input("Replacement Value")]
    public InArgument<string> ReplacementValue { get; set; }

    //Off (the default) matches capital and small letters exactly; on treats "Hello" and "hello" as the same
    [Input("Ignore Case")]
    public InArgument<bool> IgnoreCase { get; set; }

    [Output("Replaced String")]
    public OutArgument<string> ReplacedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToSearch = StringToSearch.Get(context) ?? "";
        string valueToReplace = ValueToReplace.Get(context);
        string replacementValue = ReplacementValue.Get(context);
        bool ignoreCase = IgnoreCase.Get(context);

        if (string.IsNullOrEmpty(replacementValue))
            replacementValue = "";

        //Nothing to look for means nothing to replace
        if (string.IsNullOrEmpty(valueToReplace))
        {
            ReplacedString.Set(context, stringToSearch);
            return;
        }

        string replacedString = ignoreCase
            ? ReplaceIgnoringCase(stringToSearch, valueToReplace, replacementValue)
            : stringToSearch.Replace(valueToReplace, replacementValue);

        ReplacedString.Set(context, replacedString);
    }

    //A plain search that ignores capital/small letters (no pattern rules, so "$" and "." mean themselves)
    private static string ReplaceIgnoringCase(string text, string oldValue, string newValue)
    {
        var result = new StringBuilder();
        int position = 0;
        int found = text.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while (found >= 0)
        {
            result.Append(text, position, found - position).Append(newValue);
            position = found + oldValue.Length;
            found = text.IndexOf(oldValue, position, StringComparison.OrdinalIgnoreCase);
        }

        return result.Append(text, position, text.Length - position).ToString();
    }
}
