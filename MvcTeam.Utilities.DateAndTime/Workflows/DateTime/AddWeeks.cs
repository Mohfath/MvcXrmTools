using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_AddWeeks : WorkFlowActivityBase
{
    public DateTime_AddWeeks() : base(typeof(DateTime_AddWeeks)) { }

    [RequiredArgument]
    [Input("Original Date")]
    public InArgument<DateTime> OriginalDate { get; set; }

    [RequiredArgument]
    [Input("Weeks To Add")]
    public InArgument<int> WeeksToAdd { get; set; }

    [Output("Updated Date")]
    public OutArgument<DateTime> UpdatedDate { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime originalDate = OriginalDate.Get(context);
        int weeksToAdd = WeeksToAdd.Get(context);

        DateTime updatedDate = originalDate.AddDays(weeksToAdd * 7);

        UpdatedDate.Set(context, updatedDate);
    }
}
