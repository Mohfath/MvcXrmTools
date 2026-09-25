using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Globalization;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_ToTitleCase : WorkFlowActivityBase
{
    public String_ToTitleCase() : base(typeof(String_ToTitleCase)) { }

    [RequiredArgument]
    [Input("String To Title Case")]
    public InArgument<string> StringToTitleCase { get; set; }

    [Output("Title Cased String")]
    public OutArgument<string> TitleCasedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToTitleCase = StringToTitleCase.Get(context);

        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

        string titleCasedString = ti.ToTitleCase(stringToTitleCase);

        TitleCasedString.Set(context, titleCasedString);
    }
}
