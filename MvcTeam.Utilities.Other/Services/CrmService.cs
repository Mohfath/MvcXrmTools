using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace MvcTeam.Utilities.Services
{

    public class CrmService
    {
        private IOrganizationService _orgService;
        private ITracingService _tracingService;

        public CrmService()
        {
        }

        public CrmService(IOrganizationService service, ITracingService tracingService)
        {
            _orgService = service;
            _tracingService = tracingService;
        }
        public CrmService(ApplicationConfig config)
        {
            ClientCredentials credentials = new ClientCredentials();
            if (config.UseCurrent)
            {
                credentials.Windows.ClientCredential = new System.Net.NetworkCredential();
            }
            else
            {
                credentials.Windows.ClientCredential = new System.Net.NetworkCredential(config.CrmUsername, config.CrmPassword, config.CrmDomain);
            }

            _orgService = new OrganizationServiceProxy(new Uri(config.CrmServiceUrl), null, credentials, null);
        }

        public void DeleteItem(EntityObject item)
        {
            _orgService.Delete(item.Entity.LogicalName, item.Id);
        }


        public void UpdateEntity(EntityObject item)
        {
            _orgService.Update(item.Entity);
        }

    }
}

