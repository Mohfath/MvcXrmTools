using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MvcTeam.Utilities.Workflows
{
    public class CcTeam : WorkFlowActivityBase
    {
        public CcTeam() : base(typeof(CcTeam)) { }

        [RequiredArgument]
        [Input("Email To Send")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailToSend { get; set; }

        [RequiredArgument]
        [Input("CC Team")]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> RecipientTeam { get; set; }

        [RequiredArgument]
        [Default("false")]
        [Input("Send Email?")]
        public InArgument<bool> SendEmail { get; set; }

        [Output("Users Added")]
        public OutArgument<int> UsersAdded { get; set; }

        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            EntityReference emailToSend = EmailToSend.Get(context);
            EntityReference recipientTeam = RecipientTeam.Get(context);
            bool sendEmail = SendEmail.Get(context);

            List<Entity> ccList = new List<Entity>();

            Entity email = RetrieveEmail(localContext.OrganizationService, emailToSend.Id);

            if (email == null)
            {
                UsersAdded.Set(context, 0);
                return;
            }

            //Add any pre-defined recipients specified to the array               
            foreach (Entity activityParty in email.GetAttributeValue<EntityCollection>("cc").Entities)
            {
                ccList.Add(activityParty);
            }

            EntityCollection teamMembers = GetTeamMembers(localContext.OrganizationService, recipientTeam.Id);

            ccList = ProcessUsers(localContext.OrganizationService, teamMembers, ccList);

            //Update the email
            email["cc"] = ccList.ToArray();
            localContext.OrganizationService.Update(email);

            //Send
            if (sendEmail)
            {
                SendEmailRequest request = new SendEmailRequest
                {
                    EmailId = emailToSend.Id,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };

                localContext.OrganizationService.Execute(request);
            }

            UsersAdded.Set(context, ccList.Count);
        }

        private static Entity RetrieveEmail(IOrganizationService service, Guid emailId)
        {
            return service.Retrieve("email", emailId, new ColumnSet("cc"));
        }

        private static List<Entity> ProcessUsers(IOrganizationService service, EntityCollection teamMembers, List<Entity> ccList)
        {
            foreach (Entity e in teamMembers.Entities)
            {
                Entity user = service.Retrieve("systemuser", e.GetAttributeValue<Guid>("systemuserid"),
                    new ColumnSet("internalemailaddress", "isdisabled"));

                if (string.IsNullOrEmpty(user.GetAttributeValue<string>("internalemailaddress"))) continue;
                if (user.GetAttributeValue<bool>("isdisabled")) continue;


                Entity activityParty = new Entity("activityparty")
                {
                    ["partyid"] = new EntityReference("systemuser", e.GetAttributeValue<Guid>("systemuserid"))
                };

                if (ccList.Any(t => t.GetAttributeValue<EntityReference>("partyid").Id == e.GetAttributeValue<Guid>("systemuserid"))) continue;

                ccList.Add(activityParty);
            }

            return ccList;
        }

        private static EntityCollection GetTeamMembers(IOrganizationService service, Guid teamId)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "teammembership",
                ColumnSet = new ColumnSet(true),
                LinkEntities =
                    {
                        new LinkEntity
                        {
                            LinkFromEntityName = "teammembership",
                            LinkFromAttributeName = "teamid",
                            LinkToEntityName = "team",
                            LinkToAttributeName = "teamid",
                            LinkCriteria = new FilterExpression
                            {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "teamid",
                                        Operator = ConditionOperator.Equal,
                                        Values = { teamId }
                                    }
                                }
                            }
                        }
                    }
            };

            return service.RetrieveMultiple(query);
        }
    }
}