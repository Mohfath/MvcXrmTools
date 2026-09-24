using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;

namespace MvcTeam.Utilities.Workflows
{
    public class AddBusinessDays : CodeActivity
    {
        [Input("DateTime Value")]
        [RequiredArgument]
        public InArgument<DateTime> DateTimeValue { get; set; }

        //Negative values count backwards
        [Input("Days to Add")]
        [RequiredArgument]
        public InArgument<int> DaysToAdd { get; set; }

        //Day numbers separated by '|': 0 = Sunday ... 6 = Saturday
        [Input("Weekend Days")]
        [RequiredArgument]
        [Default("4|5")]
        public InArgument<string> WeekendDays { get; set; }

        //Every row's "holiday" column (plain or aliased) is a holiday
        [Input("Holidays Query (FetchXml)")]
        public InArgument<string> HolidaysQuery { get; set; }

        [Output("Resulting DateTime")]
        public OutArgument<DateTime> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var input = DateTimeValue.Get(context);
            if (input == DateTime.MinValue) throw new InvalidPluginExecutionException("DateTime Value is required.");

            var days = DaysToAdd.Get(context);
            var weekendDays = BusinessDayCalculator.ParseWeekendDays(WeekendDays.Get(context));

            ITracingService tracingService = context.GetExtension<ITracingService>();

            //Days are counted in Iran time, so weekdays and holidays match the calendar people see
            var holidays = new HashSet<DateTime>();
            var holidaysQuery = HolidaysQuery.Get(context);
            if (!string.IsNullOrWhiteSpace(holidaysQuery))
            {
                IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
                //System service, so the holiday list doesn't depend on what the running user can read
                IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

                holidays = BusinessDayCalculator.ToIranDates(new CrmService(systemService, tracingService).GetHolidayDates(holidaysQuery));
            }

            var localResult = BusinessDayCalculator.AddBusinessDays(IranTime.FromUtc(input), days, weekendDays, holidays);
            var result = IranTime.ToUtc(localResult);

            tracingService.Trace("Added {0} business day(s) to {1:u}: {2:u} ({3} holiday(s))", days, input, result, holidays.Count);
            Result.Set(context, result);
        }
    }
}
