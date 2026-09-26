using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class Note_UpdateNoteText : WorkFlowActivityBase
{
    public Note_UpdateNoteText() : base(typeof(Note_UpdateNoteText)) { }

    [RequiredArgument]
    [Input("Note To Update")]
    [ReferenceTarget("annotation")]
    public InArgument<EntityReference> NoteToUpdate { get; set; }

    [RequiredArgument]
    [Input("New Text")]
    public InArgument<string> NewText { get; set; }

    [Output("Updated Text")]
    public OutArgument<string> UpdatedText { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference noteToUpdate = NoteToUpdate.Get(context);
        if (noteToUpdate == null)
            throw new ArgumentNullException("Note cannot be null");

        string newText = NewText.Get(context);

        Entity note = new Entity("annotation")
        {
            Id = noteToUpdate.Id,
            ["notetext"] = newText
        };
        localContext.OrganizationService.Update(note);

        UpdatedText.Set(context, newText);
    }
}
