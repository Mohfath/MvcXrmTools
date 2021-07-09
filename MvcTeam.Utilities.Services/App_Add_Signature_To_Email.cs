using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MvcTeam.Utilities.Services
{
    public class App_Add_Signature_To_Email
    {

        private IOrganizationService _organizationService;
        private ITracingService _tracingService;
        private CrmService _crmService;
        public App_Add_Signature_To_Email(IOrganizationService service, ITracingService tracingService)
        {
            _organizationService = service;
            this._tracingService = tracingService;
            _crmService = new CrmService(_organizationService, _tracingService);
        }

        public string StartFunction(Guid id, Guid userId)
        {

            //ابتدا خود ایمیل را دریافت میکنیم
            CrmEmail email = new CrmEmail(_organizationService.Retrieve("email", id, new ColumnSet(true)));

            //چک میکنیم روی آن نام کدام امضا نوشته شده است
            EmailSignatureTypes signatureType = email.SignatureType;



            //امضای مورد نظر را دریافت میکنیم
            List<CrmEmailSignature> signatures = _crmService.GetEmailSignatureForUser(userId);
            _tracingService.Trace("Total Signatures  = " + signatures.Count());

            var selectedSignature = signatures.SingleOrDefault(x => x.SignatureType == signatureType);

            if (selectedSignature == null && signatureType != EmailSignatureTypes.Null)
            {
                _tracingService.Trace("No Signature Was Found");
                throw new Exception("No Signature Was Found");
            }

            _tracingService.Trace("SelectedSignature = " + selectedSignature.Title);
            //متن امضا را استخراج میکنیم
            XmlDocument doc = new XmlDocument();
            string emailSignatureValue = string.Empty;
            //Check whether the field contains value
            if (selectedSignature.PresentationXml != null)
            {
                doc.LoadXml(Convert.ToString(selectedSignature.PresentationXml));
                emailSignatureValue = doc.SelectSingleNode("emailsignature/presentationxml").InnerText;
            }


            //این امضا را به انتهای متن ایمیل اضافه میکنیم
            email.Body += Environment.NewLine + emailSignatureValue;
            return email.Body;
        }
    }
}
