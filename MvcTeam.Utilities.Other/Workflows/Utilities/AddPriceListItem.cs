using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Models;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Utilities_AddPriceListItem : CodeActivity
{
    protected override void Execute(CodeActivityContext executionContext)
    {
        try
        {
            // Create the context and tracing service
            IExecutionContext context = executionContext.GetExtension<IExecutionContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            if (tracingService == null)
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");

            tracingService.Trace($"PriceLevel = {PriceLevel.Get(executionContext)}");
            tracingService.Trace($"Product = {Product.Get(executionContext)}");
            tracingService.Trace($"UomParam = {UomParam.Get(executionContext)}");
            tracingService.Trace($"Price = {Price.Get(executionContext)}");

            var crmService = new CrmService(service, tracingService);

            EntityReference _pricelevel = PriceLevel.Get(executionContext);
            EntityReference _product = Product.Get(executionContext);
            EntityReference _uom = UomParam.Get(executionContext);
            decimal _price = Price.Get(executionContext);
            tracingService.Trace($"Params Received");


            var priceListItem = new PriceListItem()
            {
                Price = new Money(_price),
                PriceListId = _pricelevel.Id,
                ProductId = _product.Id,
                UomId = _uom.Id
            };
            try
            {
                tracingService.Trace($"Going to create");
                service.Create(priceListItem.Entity);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error creating Pricelist Item: " + ex.Message, ex);
            }


            // All done
            tracingService.Trace("CurrentDateWorkflow.Execute() Complete. Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);
        }
        catch (Exception ex)
        {
            throw new InvalidPluginExecutionException(String.Format("An error occurred in the {0} plug-in: {1}",
                    this.GetType().ToString(), ex.Message),
                    ex);
        }
    }


    [Input("Product")]
    [ReferenceTarget("product")]
    public InArgument<EntityReference> Product { get; set; }

    [Input("Price List")]
    [ReferenceTarget("pricelevel")]
    public InArgument<EntityReference> PriceLevel { get; set; }


    [Input("UOM")]
    [ReferenceTarget("uom")]
    public InArgument<EntityReference> UomParam { get; set; }

    [Input("Price")]
    public InArgument<decimal> Price { get; set; }

}
