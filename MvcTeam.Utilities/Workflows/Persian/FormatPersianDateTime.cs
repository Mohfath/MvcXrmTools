using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    public class FormatPersianDateTime : CodeActivity
    {
        [Input("DateTime Value")]
        [RequiredArgument]
        public InArgument<DateTime> DateTimeValue { get; set; }

        //e.g. "yyyy/MM/dd" or "dddd d MMMM yyyy 'ساعت' HH:mm"; see PersianDateFormatter for all patterns
        [Input("Format")]
        [RequiredArgument]
        public InArgument<string> Format { get; set; }

        [Input("Use Persian Digits")]
        [Default("True")]
        public InArgument<bool> UsePersianDigits { get; set; }

        [Output("Formatted Value")]
        public OutArgument<string> FormattedDateTimeValue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var value = DateTimeValue.Get(context);
            if (value == DateTime.MinValue) throw new InvalidPluginExecutionException("DateTime Value is required.");
            var format = Format.Get(context);
            if (string.IsNullOrEmpty(format)) throw new InvalidPluginExecutionException("Format is required.");

            //Shown in Iran time, so the date matches the calendar people see
            var formatted = PersianDateFormatter.Format(IranTime.FromUtc(value), format, UsePersianDigits.Get(context));

            context.GetExtension<ITracingService>().Trace("Formatted {0:u} with '{1}': {2}", value, format, formatted);
            FormattedDateTimeValue.Set(context, formatted);
        }
    }
}
