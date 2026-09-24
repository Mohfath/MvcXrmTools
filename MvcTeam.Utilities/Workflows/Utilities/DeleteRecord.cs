using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.Workflows
{
    public class DeleteRecord : CodeActivity
    {


        [RequiredArgument]
        [Input("Delete Using Record URL")]
        [Default("True")]
        public InArgument<bool> DeleteUsingRecordURL { get; set; }

        [Input("Record URL")]
        [ReferenceTarget("")]
        public InArgument<String> DeleteRecordURL { get; set; }

        [Input("Entity Type Name")]
        [ReferenceTarget("")]
        public InArgument<String> EntityTypeName { get; set; }

        [Input("Entity Guid")]
        [ReferenceTarget("")]
        
        public InArgument<String> EntityGuid { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IExecutionContext executionContext = context.GetExtension<IExecutionContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(executionContext.UserId);
            ITracingService tracingService = context.GetExtension<ITracingService>();
            if (tracingService == null)
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");

            #region "Read Parameters"
            String _deleteRecordURL = this.DeleteRecordURL.Get(context);
            string entityName="";
            Guid objectId= Guid.Empty;
            

            bool _deleteUsingRecordURL = this.DeleteUsingRecordURL.Get(context);
            String _entityTypeName = this.EntityTypeName.Get(context);
            String _entityGuid = this.EntityGuid.Get(context);

            #endregion

            #region "Delete Record Execution"

            if (_deleteUsingRecordURL)
            {

                if (_deleteRecordURL == null || _deleteRecordURL == "")
                {
                    throw new InvalidOperationException("ERROR: Delete Record URL to be deleted missing.");
                }
                var parser = new DynamicUrlParser(_deleteRecordURL);
                entityName = parser.GetEntityLogicalName(service);
                objectId = parser.Id;

                service.Delete(entityName, objectId);
            }
            else
            {
                if (_entityTypeName == null || _entityTypeName == "" || _entityGuid == null || _entityGuid == "")
                {
                    throw new InvalidOperationException("ERROR: Entity Type name or GUID to be deleted missing.");
                }
                service.Delete(_entityTypeName, new Guid(_entityGuid));
            }


            #endregion

        }
    }
}
