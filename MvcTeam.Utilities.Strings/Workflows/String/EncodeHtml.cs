using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Net;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class String_EncodeHtml : WorkFlowActivityBase
{
    public String_EncodeHtml() : base(typeof(String_EncodeHtml)) { }

    [RequiredArgument]
    [Input("String To Encode")]
    public InArgument<string> StringToEncode { get; set; }

    [Output("Encoded String")]
    public OutArgument<string> EncodedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string strToEncode = StringToEncode.Get(context);
        string encodedString = WebUtility.HtmlEncode(strToEncode);

        EncodedString.Set(context, encodedString);
    }
}
