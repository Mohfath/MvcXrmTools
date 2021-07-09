using MD.PersianDateTime;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.PersianCalendar.Workflows
{


    public class GetDatePersianParts : CodeActivity
    {
        //Time: 1 hrs

        [Input("تاریخ ورودی (اگر خالی باشد اکنون)")]
        [ReferenceTarget("datetime")]
        public InArgument<DateTime> InputDate { get; set; }

        [Output("خروجی متنی کوتاه")]
        public OutArgument<string> OutputShortDate { get; set; }

        [Output("خروجی متنی بلند")]
        public OutArgument<string> OutputLongDate { get; set; }


        [Output("خروجی متنی با ساعت")]
        public OutArgument<string> OutputLongDateTime { get; set; }

        [Output("نام فارسی ماه")]
        public OutArgument<string> OutputMonthName { get; set; }

        [Output("شماره ماه شمسی")]
        public OutArgument<int> OutputMonthNumber { get; set; }

        [Output("نام روز")]
        public OutArgument<string> OutputDayName { get; set; }

        [Output("شماره روز")]
        public OutArgument<int> OutputDayNumber { get; set; }

        [Output("شماره سال شمسی")]
        public OutArgument<int> OutputYearNumber { get; set; }

        [Output("شماره فصل")]
        public OutArgument<int> OutputQuarterNumber { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var inputDate = InputDate.Get(context);
            if (inputDate == null) inputDate = DateTime.Now;


            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            // Use the context service to create an instance of IOrganizationService.             
            IOrganizationService _service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
            ITracingService tracingService = context.GetExtension<ITracingService>();
            tracingService.Trace("شیئ داده ما دریافت شد");


            try
            {
                var pd = new PersianDateTime(inputDate);
                //خروجی تاریخ شمسی
                OutputShortDate.Set(context, pd.ToShortDateString());

                //خروجی تاریخ شمسی بلند
                OutputLongDate.Set(context, pd.ToLongDateString());

                //خروجی تاریخ و ساعت شمسی بلند
                OutputLongDateTime.Set(context, pd.ToLongDateTimeString());
                
                //شماره ماه شمسی
                OutputMonthNumber.Set(context, pd.Month);

                //نام ماه فارسی
                OutputMonthName.Set(context, pd.MonthName);

                //نام روز
                OutputDayName.Set(context, pd.PersianDayOfWeek.ToString());

                //شماره روز
                OutputDayNumber.Set(context, pd.Day);

                //شماره سال
                OutputYearNumber.Set(context, pd.Year);

                //شماره فصل
                switch (pd.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        OutputQuarterNumber.Set(context, 1);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        OutputQuarterNumber.Set(context, 2);
                        break;
                    case 7:
                    case 8:
                    case 9:
                        OutputQuarterNumber.Set(context, 3);
                        break;
                    case 10:
                    case 11:
                    case 12:
                        OutputQuarterNumber.Set(context, 4);
                        break;
                }
                OutputQuarterNumber.Set(context, 1);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }


    }

}
