using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class String_Substring : WorkFlowActivityBase
{
    public String_Substring() : base(typeof(String_Substring)) { }

    [RequiredArgument]
    [Input("String To Parse")]
    public InArgument<string> StringToParse { get; set; }

    [RequiredArgument]
    [Input("Start Position")]
    public InArgument<int> StartPosition { get; set; }

    [Input("Length")]
    public InArgument<int> Length { get; set; }

    [Output("Partial String")]
    public OutArgument<string> PartialString { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        string stringToParse = StringToParse.Get(context);
        int startPosition = StartPosition.Get(context);
        int length = Length.Get(context);

        if (startPosition < 0)
            startPosition = 0;

        if (startPosition > stringToParse.Length)
        {
            localContext.TracingService.Trace("Specified start position [" + startPosition + "] is after end is string [" + stringToParse + "]");
            PartialString.Set(context, null);
            return;
        }

        if (length < 0)
            throw new InvalidPluginExecutionException("Length can't be negative.");

        //No length means "to the end"; a length that runs past the end is cut at the end
        string partialString = (length == 0 || startPosition + length > stringToParse.Length)
            ? stringToParse.Substring(startPosition)
            : stringToParse.Substring(startPosition, length);

        PartialString.Set(context, partialString);
    }
}
