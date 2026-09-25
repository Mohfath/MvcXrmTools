using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_GetQuarterStartEnd : WorkFlowActivityBase
{
    public DateTime_GetQuarterStartEnd() : base(typeof(DateTime_GetQuarterStartEnd)) { }

    [RequiredArgument]
    [Input("Date To Use")]
    public InArgument<DateTime> DateToUse { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Output("Quarter Start Date")]
    public OutArgument<DateTime> QuarterStartDate { get; set; }

    [Output("Quarter End Date")]
    public OutArgument<DateTime> QuarterEndDate { get; set; }

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

        int quarterNumber = (dateToUse.Month - 1) / 3 + 1;
        DateTime quarterStartDate = new DateTime(dateToUse.Year, (quarterNumber - 1) * 3 + 1, 1, 0, 0, 0);
        DateTime quarterEndDate = quarterStartDate.AddMonths(3).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

        QuarterStartDate.Set(context, quarterStartDate);
        QuarterEndDate.Set(context, quarterEndDate);
    }
}
