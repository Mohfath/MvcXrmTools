using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Text;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Email_DeleteEmailAttachment : WorkFlowActivityBase
{
    public Email_DeleteEmailAttachment() : base(typeof(Email_DeleteEmailAttachment)) { }

    [RequiredArgument]
    [Input("Email With Attachments To Remove")]
    [ReferenceTarget("email")]
    public InArgument<EntityReference> EmailWithAttachments { get; set; }

    [Input("Delete >= Than X Bytes (Empty = 2,147,483,647)")]
    public InArgument<int> DeleteSizeMax { get; set; }

    [Input("Delete <= Than X Bytes (Empty = 0)")]
    public InArgument<int> DeleteSizeMin { get; set; }

    [Input("Limit To Extensions (Comma Delimited, Empty = Ignore)")]
    public InArgument<string> Extensions { get; set; }

    [RequiredArgument]
    [Input("Add Delete Notice As Note?")]
    [Default("false")]
    public InArgument<bool> AppendNotice { get; set; }

    [Output("Number Of Attachments Deleted")]
    public OutArgument<int> NumberOfAttachmentsDeleted { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference emailWithAttachments = EmailWithAttachments.Get(context);
        int deleteSizeMax = DeleteSizeMax.Get(context);
        int deleteSizeMin = DeleteSizeMin.Get(context);
        string extensions = Extensions.Get(context);
        bool appendNotice = AppendNotice.Get(context);

        if (deleteSizeMax == 0) deleteSizeMax = int.MaxValue;
        if (deleteSizeMin > deleteSizeMax)
        {
            localContext.TracingService.Trace("Exception: {0}", "Min:" + deleteSizeMin + " Max:" + deleteSizeMax);
            throw new InvalidPluginExecutionException("Minimum Size Cannot Be Greater Than Maximum Size");
        }

        EntityCollection attachments = GetAttachments(localContext.OrganizationService, emailWithAttachments.Id);
        if (attachments.Entities.Count == 0) return;

        string[] filetypes = new string[0];
        if (!string.IsNullOrEmpty(extensions))
            filetypes = extensions.Replace(".", string.Empty).Split(',');

        StringBuilder notice = new StringBuilder();
        int numberOfAttachmentsDeleted = 0;

        bool delete = false;
        foreach (Entity activityMineAttachment in attachments.Entities)
        {
            delete = false;

            if (activityMineAttachment.GetAttributeValue<int>("filesize") >= deleteSizeMax)
                delete = true;

            if (activityMineAttachment.GetAttributeValue<int>("filesize") <= deleteSizeMin)
                delete = true;

            if (filetypes.Length > 0 && delete)
                delete = ExtensionMatch(filetypes, activityMineAttachment.GetAttributeValue<string>("filename"));

            if (!delete) continue;

            RemoveEmailAttachment(localContext.OrganizationService, activityMineAttachment.Id);
            numberOfAttachmentsDeleted++;

            if (appendNotice)
                notice.AppendLine("Deleted Attachment: " + activityMineAttachment.GetAttributeValue<string>("filename") + " " +
                                  DateTime.Now.ToShortDateString() + "\r\n");
        }

        if (delete && appendNotice && notice.Length > 0)
            UpdateEmail(localContext.OrganizationService, emailWithAttachments.Id, notice.ToString());

        NumberOfAttachmentsDeleted.Set(context, numberOfAttachmentsDeleted);
    }

    private static EntityCollection GetAttachments(IOrganizationService service, Guid emailId)
    {
        QueryExpression query = new QueryExpression
        {
            EntityName = "activitymimeattachment",
            ColumnSet = new ColumnSet("filesize", "filename"),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = "objectid",
                        Operator = ConditionOperator.Equal,
                        Values = { emailId }
                    }
                }
            }
        };

        return service.RetrieveMultiple(query);
    }

    private static void RemoveEmailAttachment(IOrganizationService service, Guid activitymimeattachmentId)
    {
        service.Delete("activitymimeattachment", activitymimeattachmentId);
    }

    private static void UpdateEmail(IOrganizationService service, Guid emailId, string notice)
    {
        Entity note = new Entity("annotation")
        {
            ["notetext"] = notice,
            ["objectid"] = new EntityReference("email", emailId)
        };

        service.Create(note);
    }

    private static bool ExtensionMatch(IEnumerable<string> extenstons, string filename)
    {
        foreach (string ex in extenstons)
        {
            if (filename.EndsWith("." + ex))
                return true;
        }
        return false;
    }
}
