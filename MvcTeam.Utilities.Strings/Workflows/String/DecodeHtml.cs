using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Net;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class String_DecodeHtml : WorkFlowActivityBase
{
    public String_DecodeHtml() : base(typeof(String_DecodeHtml)) { }

    [RequiredArgument]
    [Input("String To Decode")]
    public InArgument<string> StringToDecode { get; set; }

    [Output("Decoded String")]
    public OutArgument<string> DecodedString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string strToDecode = StringToDecode.Get(context);
        string decodedString = WebUtility.HtmlDecode(strToDecode);

        DecodedString.Set(context, decodedString);
    }
}
