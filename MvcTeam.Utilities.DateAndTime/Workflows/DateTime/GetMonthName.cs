using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Globalization;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_GetMonthName : WorkFlowActivityBase
{
    public DateTime_GetMonthName() : base(typeof(DateTime_GetMonthName)) { }

    [RequiredArgument]
    [Input("Date To Use")]
    public InArgument<DateTime> DateToUse { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Input("Culture")]
    [Default("en-US")]
    public InArgument<string> Culture { get; set; }

    [Output("Month Name")]
    public OutArgument<string> MonthName { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime dateToUse = DateToUse.Get(context);
        bool evaluateAsUserLocal = EvaluateAsUserLocal.Get(context);
        string culture = Culture.Get(context);

        if (evaluateAsUserLocal)
        {
            int? timeZoneCode = GetLocalTime.RetrieveTimeZoneCode(localContext.OrganizationService);
            dateToUse = GetLocalTime.RetrieveLocalTimeFromUtcTime(dateToUse, timeZoneCode, localContext.OrganizationService);
        }

        string monthName = dateToUse.ToString("MMMM", CultureInfo.CreateSpecificCulture(culture));

        MonthName.Set(context, monthName);
    }
}
