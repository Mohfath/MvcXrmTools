using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class Note_DeleteNote : WorkFlowActivityBase
{
    public Note_DeleteNote() : base(typeof(Note_DeleteNote)) { }

    [RequiredArgument]
    [Input("Note To Delete")]
    [ReferenceTarget("annotation")]
    public InArgument<EntityReference> NoteToDelete { get; set; }

    [Output("Was Note Deleted")]
    public OutArgument<bool> WasNoteDeleted { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference noteToDelete = NoteToDelete.Get(context);
        if (noteToDelete == null)
            throw new ArgumentNullException("Note cannot be null");

        localContext.OrganizationService.Delete("annotation", noteToDelete.Id);

        WasNoteDeleted.Set(context, true);
    }
}
