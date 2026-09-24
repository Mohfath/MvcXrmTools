using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MvcTeam.Utilities.Workflows
{
    public class SendEmail : WorkFlowActivityBase
    {
        public SendEmail() : base(typeof(SendEmail)) { }

        [RequiredArgument]
        [Input("Email To Send")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailToSend { get; set; }

        [Output("Email Sent")]
        public OutArgument<bool> EmailSent { get; set; }

        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            EntityReference emailToSend = EmailToSend.Get(context);

            SendEmailRequest request = new SendEmailRequest
            {
                EmailId = emailToSend.Id,
                TrackingToken = string.Empty,
                IssueSend = true
            };

            SendEmailResponse response = (SendEmailResponse)localContext.OrganizationService.Execute(request);

            bool emailSent = response != null;

            EmailSent.Set(context, emailSent);
        }
    }
}