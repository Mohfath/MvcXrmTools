using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_GetHourNumber : WorkFlowActivityBase
{
    public DateTime_GetHourNumber() : base(typeof(DateTime_GetHourNumber)) { }

    [RequiredArgument]
    [Input("Date To Use")]
    public InArgument<DateTime> DateToUse { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Output("Hour Number")]
    public OutArgument<int> HourNumber { get; set; }

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

        int hourNumber = dateToUse.Hour;

        HourNumber.Set(context, hourNumber);
    }
}
