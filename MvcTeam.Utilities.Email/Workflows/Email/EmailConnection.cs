using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Email_EmailConnection : WorkFlowActivityBase
{
    public Email_EmailConnection() : base(typeof(Email_EmailConnection)) { }

    [RequiredArgument]
    [Input("Email To Send")]
    [ReferenceTarget("email")]
    public InArgument<EntityReference> EmailToSend { get; set; }

    [RequiredArgument]
    [Input("Connection Role")]
    [ReferenceTarget("connectionrole")]
    public InArgument<EntityReference> ConnectionRole { get; set; }

    [RequiredArgument]
    [Default("false")]
    [Input("Include Opposite Connection?")]
    public InArgument<bool> IncludeOppositeConnection { get; set; }

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

        Guid primaryEntityId = localContext.WorkflowExecutionContext.PrimaryEntityId;
        EntityReference emailToSend = EmailToSend.Get(context);
        EntityReference connectionRole = ConnectionRole.Get(context);
        bool includeOpposite = IncludeOppositeConnection.Get(context);
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

        EntityCollection records = GetConnectedRecords(localContext.OrganizationService, primaryEntityId, connectionRole.Id, includeOpposite);

        toList = ProcessRecords(localContext.OrganizationService, records, toList, primaryEntityId);

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

    private static Entity GetEntity(IOrganizationService service, string logicalName, Guid id, string emailField)
    {
        ColumnSet columnSet = new ColumnSet();
        columnSet.AddColumn(emailField);
        columnSet.AddColumn(logicalName == "systemuser" ? "isdisabled" : "statecode");

        return service.Retrieve(logicalName, id, columnSet);
    }

    private static List<Entity> ProcessRecords(IOrganizationService service, EntityCollection records, List<Entity> toList, Guid primaryEntityId)
    {
        foreach (Entity e in records.Entities)
        {
            Guid id = GetEntityId(e, primaryEntityId);

            var typeCode = GetTypeCode(e, primaryEntityId);

            string emailField = GetEmailField(typeCode);

            string logicalName = GetEntityLogicalName(service, typeCode);

            Entity entity = GetEntity(service, logicalName, id, emailField);

            bool hasEmail = entity.Attributes.TryGetValue(emailField, out object emailObj);
            if (!hasEmail)
                continue;
            if (string.IsNullOrEmpty(emailObj.ToString()))
                continue;

            if (entity.LogicalName == "systemuser")
            {
                if (entity.GetAttributeValue<bool>("isdisabled"))
                    continue;
            }
            else
            {
                if (entity.GetAttributeValue<OptionSetValue>("statecode").Value != 0)
                    continue;
            }

            Entity activityParty =
                new Entity("activityparty")
                {
                    ["partyid"] = new EntityReference(logicalName, id)
                };

            if (toList.Any(t => t.GetAttributeValue<EntityReference>("partyid").Id == id)) continue;

            toList.Add(activityParty);
        }

        return toList;
    }

    private static Guid GetEntityId(Entity e, Guid primaryEntityId)
    {
        Guid record1Id = e.GetAttributeValue<EntityReference>("record1id").Id;
        Guid id = record1Id == primaryEntityId
            ? e.GetAttributeValue<EntityReference>("record2id").Id
            : record1Id;

        return id;
    }

    private static string GetEmailField(int typeCode)
    {
        string emailField;
        switch (typeCode)
        {
            case 8:
                emailField = "internalemailaddress";
                break;
            case 1:
            case 2:
            case 4:
                emailField = "emailaddress1";
                break;
            default:
                emailField = "emailaddress";
                break;
        }

        return emailField;
    }

    private static int GetTypeCode(Entity e, Guid primaryEntityId)
    {
        Guid record1Id = e.GetAttributeValue<EntityReference>("record1id").Id;
        int typeCode = record1Id != primaryEntityId
            ? e.GetAttributeValue<OptionSetValue>("record1objecttypecode").Value
            : e.GetAttributeValue<OptionSetValue>("record2objecttypecode").Value;
        return typeCode;
    }

    private static EntityCollection GetConnectedRecords(IOrganizationService service, Guid primaryEntityId, Guid connectionRoleId, bool includeOpposite)
    {
        QueryExpression query = new QueryExpression
        {
            EntityName = "connection",
            ColumnSet = new ColumnSet("connectionid", "record1id", "record2id", "record1objecttypecode",
                "record2objecttypecode"),
            Criteria = new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Filters = {
                    new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions = {
                            new ConditionExpression {
                                AttributeName = "record1id",
                                Operator = ConditionOperator.Equal,
                                Values = {primaryEntityId}
                            },
                            new ConditionExpression {
                                AttributeName = "record2id",
                                Operator = ConditionOperator.NotEqual,
                                Values = {primaryEntityId}
                            },
                            new ConditionExpression {
                                AttributeName = "record2roleid",
                                Operator = ConditionOperator.Equal,
                                Values = {connectionRoleId}
                            }
                        }
                    }
                }
            }
        };

        if (includeOpposite)
        {
            query.Criteria.Filters.Add(new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions = {
                    new ConditionExpression {
                        AttributeName = "record1id",
                        Operator = ConditionOperator.NotEqual,
                        Values = {primaryEntityId}
                    },
                    new ConditionExpression {
                        AttributeName = "record2id",
                        Operator = ConditionOperator.Equal,
                        Values = {primaryEntityId}
                    },
                    new ConditionExpression {
                        AttributeName = "record2roleid",
                        Operator = ConditionOperator.Equal,
                        Values = {connectionRoleId}
                    }
                }
            });
        }

        return service.RetrieveMultiple(query);
    }

    private static Entity RetrieveEmail(IOrganizationService service, Guid emailId)
    {
        return service.Retrieve("email", emailId, new ColumnSet("to"));
    }

    private static string GetEntityLogicalName(IOrganizationService service, int typeCode)
    {
        var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
        entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode",MetadataConditionOperator.Equals, typeCode));
        var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
        propertyExpression.PropertyNames.Add("LogicalName");
        var entityQueryExpression = new EntityQueryExpression
        {
            Criteria = entityFilter,
            Properties = propertyExpression
        };

        var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest
        {
            Query = entityQueryExpression
        };

        var response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

        return response.EntityMetadata.Count == 1
            ? response.EntityMetadata[0].LogicalName
            : null;
    }
}
