using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Email_CheckAttachments : WorkFlowActivityBase
{
    public Email_CheckAttachments() : base(typeof(Email_CheckAttachments)) { }

    [RequiredArgument]
    [Input("Email To Check")]
    [ReferenceTarget("email")]
    public InArgument<EntityReference> EmailToCheck { get; set; }

    [Output("Has Attachments")]
    public OutArgument<bool> HasAttachments { get; set; }

    [Output("Attachment Count")]
    public OutArgument<int> AttachmentCount { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference emailToCheck = EmailToCheck.Get(context);

        int count = GetAttachmentCount(localContext.OrganizationService, emailToCheck.Id);

        AttachmentCount.Set(context, count);
        HasAttachments.Set(context, count > 0);
    }

    private static int GetAttachmentCount(IOrganizationService service, Guid emailId)
    {
        FetchExpression query = new FetchExpression(@"<fetch aggregate='true' >
                                                        <entity name='email' >
                                                        <attribute name='activityid' alias='count' aggregate='count' />
                                                        <filter>
                                                            <condition entityname='am' attribute='activityid' operator='eq' value='" + emailId + @"' />
                                                        </filter>
                                                        <link-entity name='activitymimeattachment' from='activityid' to='activityid' link-type='inner' alias='am' />
                                                        </entity>
                                                    </fetch>");

        EntityCollection results = service.RetrieveMultiple(query);

        return (int)results.Entities[0].GetAttributeValue<AliasedValue>("count").Value;
    }
}
