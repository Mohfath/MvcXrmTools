using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class DateTime_AddMinutes : WorkFlowActivityBase
{
    public DateTime_AddMinutes() : base(typeof(DateTime_AddMinutes)) { }

    [RequiredArgument]
    [Input("Original Date")]
    public InArgument<DateTime> OriginalDate { get; set; }

    [RequiredArgument]
    [Input("Minutes To Add")]
    public InArgument<int> MinutesToAdd { get; set; }

    [Output("Updated Date")]
    public OutArgument<DateTime> UpdatedDate { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        DateTime originalDate = OriginalDate.Get(context);
        int minutesToAdd = MinutesToAdd.Get(context);

        DateTime updatedDate = originalDate.AddMinutes(minutesToAdd);

        UpdatedDate.Set(context, updatedDate);
    }
}
