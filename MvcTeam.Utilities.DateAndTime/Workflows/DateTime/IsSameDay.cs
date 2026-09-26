using MvcTeam.Utilities.Workflows.Common;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_IsSameDay : WorkFlowActivityBase
{
    public DateTime_IsSameDay() : base(typeof(DateTime_IsSameDay)) { }

    [RequiredArgument]
    [Input("First Date")]
    public InArgument<DateTime> FirstDate { get; set; }

    [RequiredArgument]
    [Input("Second Date")]
    public InArgument<DateTime> SecondDate { get; set; }

    [RequiredArgument]
    [Input("Evaluate As User Local")]
    [Default("True")]
    public InArgument<bool> EvaluateAsUserLocal { get; set; }

    [Output("Same Day")]
    public OutArgument<bool> SameDay { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime firstDate = FirstDate.Get(context);
        DateTime secondDate = SecondDate.Get(context);
        bool evaluateAsUserLocal = EvaluateAsUserLocal.Get(context);

        if (evaluateAsUserLocal)
        {
            int? timeZoneCode = GetLocalTime.RetrieveTimeZoneCode(localContext.OrganizationService);
            firstDate = GetLocalTime.RetrieveLocalTimeFromUtcTime(firstDate, timeZoneCode, localContext.OrganizationService);
            secondDate = GetLocalTime.RetrieveLocalTimeFromUtcTime(secondDate, timeZoneCode, localContext.OrganizationService);
        }

        bool sameDay = firstDate.Date == secondDate.Date;

        SameDay.Set(context, sameDay);
    }
}
