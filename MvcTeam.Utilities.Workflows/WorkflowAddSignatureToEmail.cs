using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MvcTeam.Utilities.Workflows
{
    public class WorkflowAddSignatureToEmail : CodeActivity
    {
        //Creating Output parameter
        [Input("Email")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> Email { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var entityReference = Email.Get(context);

            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            // Use the context service to create an instance of IOrganizationService.             
            IOrganizationService _service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
            ITracingService tracingService = context.GetExtension<ITracingService>();


            tracingService.Trace("شیئ داده ما دریافت شد");


            //کاربر فعلی را استخراج میکنیم
            var emailEntity = _service.Retrieve("email", entityReference.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("ownerid"));

            Guid userId = ((EntityReference)emailEntity["ownerid"]).Id;

            //بارگزاری موتور برنامه
            var _applicationService = new App_Add_Signature_To_Email(_service, tracingService);

            try
            {
                var newBody = _applicationService.StartFunction(entityReference.Id,userId);

                var tempEmail = new Entity("email");
                tempEmail.Attributes["description"] = newBody;
                tempEmail.Id = entityReference.Id;

                _service.Update(tempEmail);

            }

            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }

    }
}
