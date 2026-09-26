using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public sealed class Note_CheckAttachment : WorkFlowActivityBase
{
    public Note_CheckAttachment() : base(typeof(Note_CheckAttachment)) { }

    [RequiredArgument]
    [Input("Note To Check")]
    [ReferenceTarget("annotation")]
    public InArgument<EntityReference> NoteToCheck { get; set; }

    [Output("Has Attachment")]
    public OutArgument<bool> HasAttachment { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference noteToCheck = NoteToCheck.Get(context);
        if (noteToCheck == null)
            throw new ArgumentNullException("Note cannot be null");

        HasAttachment.Set(context, CheckForAttachment(localContext.OrganizationService, noteToCheck.Id));
    }

    private static bool CheckForAttachment(IOrganizationService service, Guid noteId)
    {
        Entity note = service.Retrieve("annotation", noteId, new ColumnSet("isdocument"));

        bool hasValue = note.Attributes.TryGetValue("isdocument", out var oIsDocument);
        if (!hasValue)
            return false;

        return (bool)oIsDocument;
    }
}
