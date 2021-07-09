using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MvcTeam.Utilities.Services
{
    public class App_Mark_Email_As_Read
    {

        private IOrganizationService _organizationService;
        private ITracingService _tracingService;
        private CrmService _crmService;
        public App_Mark_Email_As_Read(IOrganizationService service, ITracingService tracingService)
        {
            _organizationService = service;
            this._tracingService = tracingService;
            _crmService = new CrmService(_organizationService, _tracingService);
        }

        public void StartFunction(Guid id)
        {

            //ابتدا خود ایمیل را دریافت میکنیم
            CrmEmail email = new CrmEmail(_organizationService.Retrieve("email", id, new ColumnSet(true)));

            if (email.ReadState)
                return;

            var tempEntity = new CrmEmail();
            tempEntity.Id = id;
            tempEntity.ReadState = true;

            _crmService.UpdateEntity(tempEntity);
        }
    }
}
