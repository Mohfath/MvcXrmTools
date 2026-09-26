using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_AddDays : WorkFlowActivityBase
{
    public DateTime_AddDays() : base(typeof(DateTime_AddDays)) { }

    [RequiredArgument]
    [Input("Original Date")]
    public InArgument<DateTime> OriginalDate { get; set; }

    [RequiredArgument]
    [Input("Days To Add")]
    public InArgument<int> DaysToAdd { get; set; }

    [Output("Updated Date")]
    public OutArgument<DateTime> UpdatedDate { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime originalDate = OriginalDate.Get(context);
        int daysToAdd = DaysToAdd.Get(context);

        DateTime updatedDate = originalDate.AddDays(daysToAdd);

        UpdatedDate.Set(context, updatedDate);
    }
}
