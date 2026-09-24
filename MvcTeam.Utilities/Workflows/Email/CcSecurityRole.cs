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
    public class CcSecurityRole : WorkFlowActivityBase
    {
        public CcSecurityRole() : base(typeof(CcSecurityRole)) { }

        [RequiredArgument]
        [Input("Email To Send")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailToSend { get; set; }

        [RequiredArgument]
        [Input("Security Role GUID")]
        [ReferenceTarget("role")]
        public InArgument<string> RecipientRole { get; set; }

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
            Guid roleId = IsGuid(RecipientRole.Get(context));
            bool sendEmail = SendEmail.Get(context);

            if (roleId == Guid.Empty)
                throw new InvalidWorkflowException("Invalid Role GUID");

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

            EntityCollection users = GetRoleUsers(localContext.OrganizationService, roleId);

            ccList = ProcessUsers(users, ccList);

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

        private static List<Entity> ProcessUsers(EntityCollection users, List<Entity> ccList)
        {
            foreach (Entity e in users.Entities)
            {
                Entity activityParty =
                    new Entity("activityparty")
                    {
                        ["partyid"] = new EntityReference("systemuser", e.Id)
                    };

                if (ccList.Any(t => t.GetAttributeValue<EntityReference>("partyid").Id == e.Id)) continue;

                ccList.Add(activityParty);
            }

            return ccList;
        }

        private static EntityCollection GetRoleUsers(IOrganizationService service, Guid id)
        {
            //Query for the users with security role
            QueryExpression query = new QueryExpression
            {
                EntityName = "systemuser",
                ColumnSet = new ColumnSet("systemuserid"),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "systemuser",
                        LinkFromAttributeName = "systemuserid",
                        LinkToEntityName = "systemuserroles",
                        LinkToAttributeName = "systemuserid",
                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression
                                {
                                    AttributeName = "roleid",
                                    Operator = ConditionOperator.Equal,
                                    Values = { id }
                                }
                            }
                        }
                    }
                },
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "internalemailaddress",
                            Operator = ConditionOperator.NotNull
                        },
                        new ConditionExpression
                        {
                            AttributeName = "isdisabled",
                            Operator = ConditionOperator.Equal,
                            Values = { false }
                        }
                    }
                }
            };

            return service.RetrieveMultiple(query);
        }

        private static Entity RetrieveEmail(IOrganizationService service, Guid emailId)
        {
            return service.Retrieve("email", emailId, new ColumnSet("cc"));
        }

        private static Guid IsGuid(string value)
        {
            return Guid.TryParse(value, out var parsed)
                ? parsed
                : Guid.Empty;
        }
    }
}