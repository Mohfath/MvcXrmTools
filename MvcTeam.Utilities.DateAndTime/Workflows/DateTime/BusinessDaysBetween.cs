using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;

//Differences between two dates, counted in Iran time. Day counts exclude the start day and include the end day,
//so "Add Business Days" of N from Start Date gives End Date back N. All results are negative when End Date is before Start Date,
//except Weekend And Holiday Days, which is always a count.
public class DateTime_BusinessDaysBetween : CodeActivity
{
    [Input("Start Date")]
    [RequiredArgument]
    public InArgument<DateTime> StartDate { get; set; }

    [Input("End Date")]
    [RequiredArgument]
    public InArgument<DateTime> EndDate { get; set; }

    //Day numbers separated by '|': 0 = Sunday ... 6 = Saturday
    [Input("Weekend Days")]
    [RequiredArgument]
    [Default("4|5")]
    public InArgument<string> WeekendDays { get; set; }

    //Every row's "holiday" column (plain or aliased) is a holiday
    [Input("Holidays Query (FetchXml)")]
    public InArgument<string> HolidaysQuery { get; set; }

    [Output("Business Days")]
    public OutArgument<int> BusinessDays { get; set; }

    [Output("Calendar Days")]
    public OutArgument<int> CalendarDays { get; set; }

    [Output("Weekend And Holiday Days")]
    public OutArgument<int> SkippedDays { get; set; }

    [Output("Total Hours")]
    public OutArgument<double> TotalHours { get; set; }

    [Output("Total Minutes")]
    public OutArgument<double> TotalMinutes { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
        var start = StartDate.Get(context);
        if (start == DateTime.MinValue) throw new InvalidPluginExecutionException("Start Date is required.");
        var end = EndDate.Get(context);
        if (end == DateTime.MinValue) throw new InvalidPluginExecutionException("End Date is required.");

        var weekendDays = BusinessDayCalculator.ParseWeekendDays(WeekendDays.Get(context));
        ITracingService tracingService = context.GetExtension<ITracingService>();

        var holidays = new HashSet<DateTime>();
        var holidaysQuery = HolidaysQuery.Get(context);
        if (!string.IsNullOrWhiteSpace(holidaysQuery))
        {
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            //System service, so the holiday list doesn't depend on what the running user can read
            IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);
            holidays = BusinessDayCalculator.ToIranDates(new CrmService(systemService, tracingService).GetHolidayDates(holidaysQuery));
        }

        //Days are counted in Iran time, so weekdays and holidays match the calendar people see
        var startLocal = IranTime.FromUtc(start);
        var endLocal = IranTime.FromUtc(end);

        var calendarDays = (endLocal.Date - startLocal.Date).Days;
        var businessDays = BusinessDayCalculator.CountBusinessDays(startLocal, endLocal, weekendDays, holidays);
        var elapsed = IranTime.ToUtc(endLocal) - IranTime.ToUtc(startLocal);

        tracingService.Trace("{0:u} to {1:u}: {2} business / {3} calendar day(s)", start, end, businessDays, calendarDays);
        BusinessDays.Set(context, businessDays);
        CalendarDays.Set(context, calendarDays);
        SkippedDays.Set(context, Math.Abs(calendarDays) - Math.Abs(businessDays));
        TotalHours.Set(context, elapsed.TotalHours);
        TotalMinutes.Set(context, elapsed.TotalMinutes);
    }
}
