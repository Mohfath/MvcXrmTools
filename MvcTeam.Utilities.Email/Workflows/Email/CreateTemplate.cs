using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using MvcTeam.Utilities.Workflows;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

public class Email_CreateTemplate : WorkFlowActivityBase
{
    public Email_CreateTemplate() : base(typeof(Email_CreateTemplate)) { }

    [Input("Template")]
    [RequiredArgument]
    [ReferenceTarget("template")]
    public InArgument<EntityReference> Template { get; set; }

    [Input("Record Dynamic Url")]
    [RequiredArgument]
    public InArgument<string> RecordUrl { get; set; }

    [Output("Template Subject")]
    public OutArgument<string> TemplateSubject { get; set; }

    [Output("Template Description")]
    public OutArgument<string> TemplateDescription { get; set; }

    protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (localContext == null)
            throw new ArgumentNullException(nameof(localContext));

        EntityReference template = Template.Get(context);
        string recordUrl = RecordUrl.Get(context);

        var dup = new DynamicUrlParser(recordUrl);
        string logicalName = dup.GetEntityLogicalName(localContext.OrganizationService);

        Entity email = GenerateTemplate(localContext.OrganizationService, template.Id, dup.Id, logicalName);
        if (email == null)
            throw new InvalidPluginExecutionException("Unexpected error creating template");

        string subject = null;
        bool hasSubject = email.Attributes.TryGetValue("subject", out object objSubject);
        if (hasSubject)
            subject = objSubject.ToString();
        TemplateSubject.Set(context, subject);

        string description = null;
        bool hasDescription = email.Attributes.TryGetValue("description", out object objDescription);
        if (hasDescription)
            description = objDescription.ToString();
        TemplateDescription.Set(context, description);
    }

    private static Entity GenerateTemplate(IOrganizationService service, Guid templateId, Guid recordId, string logicalName)
    {
        try
        {
            InstantiateTemplateRequest request = new InstantiateTemplateRequest
            {
                TemplateId = templateId,
                ObjectId = recordId,
                ObjectType = logicalName
            };

            InstantiateTemplateResponse response = (InstantiateTemplateResponse)service.Execute(request);

            return response.EntityCollection.Entities.Count == 1
                ? response.EntityCollection.Entities[0]
                : null;
        }
        catch (Exception)
        {
            throw new InvalidPluginExecutionException("Template type does not match entity in record url");
        }
    }
}
