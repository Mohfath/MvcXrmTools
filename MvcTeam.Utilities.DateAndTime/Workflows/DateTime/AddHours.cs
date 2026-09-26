using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_AddHours : WorkFlowActivityBase
{
    public DateTime_AddHours() : base(typeof(DateTime_AddHours)) { }

    [RequiredArgument]
    [Input("Original Date")]
    public InArgument<DateTime> OriginalDate { get; set; }

    [RequiredArgument]
    [Input("Hours To Add")]
    public InArgument<int> HoursToAdd { get; set; }

    [Output("Updated Date")]
    public OutArgument<DateTime> UpdatedDate { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime originalDate = OriginalDate.Get(context);
        int hoursToAdd = HoursToAdd.Get(context);

        DateTime updatedDate = originalDate.AddHours(hoursToAdd);

        UpdatedDate.Set(context, updatedDate);
    }
}
