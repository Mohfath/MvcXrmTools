using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_WordCount : WorkFlowActivityBase
{
    public String_WordCount() : base(typeof(String_WordCount)) { }

    [RequiredArgument]
    [Input("String To Count")]
    public InArgument<string> StringToCount { get; set; }

    [Output("Word Count")]
    public OutArgument<int> Count { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToCount = StringToCount.Get(context);

        string[] words = stringToCount.Trim().Split(' ');

        Count.Set(context, words.Length);
    }
}
