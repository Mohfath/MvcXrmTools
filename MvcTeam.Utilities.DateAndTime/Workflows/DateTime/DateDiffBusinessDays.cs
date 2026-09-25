using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class DateTime_DateDiffBusinessDays : WorkFlowActivityBase
{
    public DateTime_DateDiffBusinessDays() : base(typeof(DateTime_DateDiffBusinessDays)) { }

    [RequiredArgument]
    [Input("Starting Date")]
    public InArgument<DateTime> StartingDate { get; set; }

    [RequiredArgument]
    [Input("Ending Date")]
    public InArgument<DateTime> EndingDate { get; set; }

    [Input("Holiday/Closure Calendar")]
    [ReferenceTarget("calendar")]
    public InArgument<EntityReference> HolidayClosureCalendar { get; set; }

    [Output("Days Difference")]
    public OutArgument<int> DaysDifference { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        var startDate = StartingDate.Get(context);
        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute, 0);
        var endDate = EndingDate.Get(context);
        endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, 0);

        if (startDate == endDate)
        {
            DaysDifference.Set(context, 0);
            return;
        }

        Entity calendar = null;
        EntityReference holidayClosureCalendar = HolidayClosureCalendar.Get(context);
        if (holidayClosureCalendar != null)
            calendar = localContext.OrganizationService.Retrieve("calendar", holidayClosureCalendar.Id, new ColumnSet(true));

        var businessMinutes = endDate > startDate
            ? BusinessMinuteLogic.CountBusinessMinutes(startDate, endDate.AddMinutes(-1), calendar)
            : -BusinessMinuteLogic.CountBusinessMinutes(endDate.AddMinutes(1), startDate, calendar);

        TimeSpan ts = TimeSpan.FromMinutes(businessMinutes);



        var businessHours = businessMinutes >= 0
            ? Math.Floor(ts.TotalDays)
            : Math.Ceiling(ts.TotalDays);

        DaysDifference.Set(context, (int)businessHours);
    }
}
