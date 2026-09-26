using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_ReplaceWithSpace : WorkFlowActivityBase
{
    public String_ReplaceWithSpace() : base(typeof(String_ReplaceWithSpace)) { }

    [RequiredArgument]
    [Input("String To Search")]
    public InArgument<string> StringToSearch { get; set; }

    [RequiredArgument]
    [Input("Value To Replace")]
    public InArgument<string> ValueToReplace { get; set; }

    [RequiredArgument]
    [Input("Number Of Spaces")]
    public InArgument<int> NumberOfSpaces { get; set; }

    [Output("Replaced String")]
    public OutArgument<string> ReplacedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToSearch = StringToSearch.Get(context);
        string valueToReplace = ValueToReplace.Get(context);
        int numberOfSpaces = NumberOfSpaces.Get(context);

        string spaces = "";
        spaces = spaces.PadRight(numberOfSpaces, ' ');

        string replacedString = stringToSearch.Replace(valueToReplace, spaces);

        ReplacedString.Set(context, replacedString);
    }
}
