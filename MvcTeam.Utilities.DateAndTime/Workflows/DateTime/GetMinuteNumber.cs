using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_GetMinuteNumber : WorkFlowActivityBase
{
    public DateTime_GetMinuteNumber() : base(typeof(DateTime_GetMinuteNumber)) { }

    [RequiredArgument]
    [Input("Date To Use")]
    public InArgument<DateTime> DateToUse { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Output("Minute Number")]
    public OutArgument<int> MinuteNumber { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime dateToUse = DateToUse.Get(context);
        bool evaluateAsUserLocal = EvaluateAsUserLocal.Get(context);

        if (evaluateAsUserLocal)
        {
            int? timeZoneCode = GetLocalTime.RetrieveTimeZoneCode(localContext.OrganizationService);
            dateToUse = GetLocalTime.RetrieveLocalTimeFromUtcTime(dateToUse, timeZoneCode, localContext.OrganizationService);
        }

        int minuteNumber = dateToUse.Minute;

        MinuteNumber.Set(context, minuteNumber);
    }
}
