using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_DateDiffDays : WorkFlowActivityBase
{
    public DateTime_DateDiffDays() : base(typeof(DateTime_DateDiffDays)) { }

    [RequiredArgument]
    [Input("Starting Date")]
    public InArgument<DateTime> StartingDate { get; set; }

    [RequiredArgument]
    [Input("Ending Date")]
    public InArgument<DateTime> EndingDate { get; set; }

    [Output("Days Difference")]
    public OutArgument<int> DaysDifference { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime startingDate = StartingDate.Get(context);
        DateTime endingDate = EndingDate.Get(context);

        TimeSpan difference = startingDate - endingDate;

        int daysDifference = (int)Math.Abs(Math.Truncate(difference.TotalDays));

        DaysDifference.Set(context, daysDifference);
    }
}
