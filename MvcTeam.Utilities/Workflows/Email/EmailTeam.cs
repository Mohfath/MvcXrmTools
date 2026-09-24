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
    public class EmailTeam : WorkFlowActivityBase
    {
        public EmailTeam() : base(typeof(EmailTeam)) { }

        [RequiredArgument]
        [Input("Email To Send")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailToSend { get; set; }

        [RequiredArgument]
        [Input("Recipient Team")]
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

            List<Entity> toList = new List<Entity>();

            Entity email = RetrieveEmail(localContext.OrganizationService, emailToSend.Id);

            if (email == null)
            {
                UsersAdded.Set(context, 0);
                return;
            }

            //Add any pre-defined recipients specified to the array               
            foreach (Entity activityParty in email.GetAttributeValue<EntityCollection>("to").Entities)
            {
                toList.Add(activityParty);
            }

            EntityCollection teamMembers = GetTeamMembers(localContext.OrganizationService, recipientTeam.Id);

            toList = ProcessUsers(localContext.OrganizationService, teamMembers, toList);

            //Update the email
            email["to"] = toList.ToArray();
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

            UsersAdded.Set(context, toList.Count);
        }

        private static Entity RetrieveEmail(IOrganizationService service, Guid emailId)
        {
            return service.Retrieve("email", emailId, new ColumnSet("to"));
        }

        private static List<Entity> ProcessUsers(IOrganizationService service, EntityCollection teamMembers, List<Entity> toList)
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

                if (toList.Any(t => t.GetAttributeValue<EntityReference>("partyid").Id == e.GetAttributeValue<Guid>("systemuserid"))) continue;

                toList.Add(activityParty);
            }

            return toList;
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