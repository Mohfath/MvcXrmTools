using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_GetDayNumberOfYear : WorkFlowActivityBase
{
    public DateTime_GetDayNumberOfYear() : base(typeof(DateTime_GetDayNumberOfYear)) { }

    [RequiredArgument]
    [Input("Date To Use")]
    public InArgument<DateTime> DateToUse { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Output("Day Number Of Year")]
    public OutArgument<int> DayNumberOfYear { get; set; }

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

        int dayNumberOfYear = dateToUse.DayOfYear;

        DayNumberOfYear.Set(context, dayNumberOfYear);
    }
}
