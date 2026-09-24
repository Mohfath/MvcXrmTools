using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MvcTeam.Utilities.Workflows
{
    public class DeleteEmailAttachmentByName : WorkFlowActivityBase
    {
        public DeleteEmailAttachmentByName() : base(typeof(DeleteEmailAttachmentByName)) { }

        [RequiredArgument]
        [Input("Email With Attachments To Remove")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailWithAttachments { get; set; }

        [RequiredArgument]
        [Input("File Name With Extension")]
        public InArgument<string> FileName { get; set; }

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
            string fileName = FileName.Get(context);
            bool appendNotice = AppendNotice.Get(context);

            EntityCollection attachments = GetAttachments(localContext.OrganizationService, emailWithAttachments.Id);
            if (attachments.Entities.Count == 0) return;

            StringBuilder notice = new StringBuilder();
            int numberOfAttachmentsDeleted = 0;

            foreach (Entity activityMineAttachment in attachments.Entities)
            {
                if (!string.Equals(activityMineAttachment.GetAttributeValue<string>("filename"), fileName, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                DeleteEmailAttachment(localContext.OrganizationService, activityMineAttachment.Id);
                numberOfAttachmentsDeleted++;

                if (appendNotice)
                    notice.AppendLine("Deleted Attachment: " + activityMineAttachment.GetAttributeValue<string>("filename") + " " +
                                      DateTime.Now.ToShortDateString() + "\r\n");
            }

            if (appendNotice && notice.Length > 0)
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

        private static void DeleteEmailAttachment(IOrganizationService service, Guid activitymimeattachmentId)
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
    }
}