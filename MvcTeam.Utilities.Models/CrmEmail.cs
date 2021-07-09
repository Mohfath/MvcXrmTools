using Microsoft.Xrm.Sdk;

namespace MvcTeam.Utilities.Models
{
    public class CrmEmail : EntityObject
    {
        public CrmEmail(Entity entity) : base(entity)
        {
        }

        public CrmEmail()
        {
            Entity = new Entity("email");
        }

        private int SignatureId { get { return string.IsNullOrEmpty(GetValue("new_signature")) ? 0 : int.Parse(GetValue("new_signature")); } }
        public EmailSignatureTypes SignatureType
        {
            get
            {
                switch (SignatureId)
                {
                    case 100: return EmailSignatureTypes.Arij;
                    case 200: return EmailSignatureTypes.Miracle;
                    case 300: return EmailSignatureTypes.ArijArabic;
                    case 400: return EmailSignatureTypes.Romak;
                    case 500: return EmailSignatureTypes.Null;
                    default: return EmailSignatureTypes.Null;
                }

            }
        }
        public string Body { get { return GetValue("description"); } set { SetValue("description", value); } }

        public string Name { get { return GetValue("invoicedetailname"); } set { SetValue("invoicedetailname", value); } }

        public bool ReadState { get { return (string.IsNullOrEmpty(GetValue("new_emailreadstatus"))) ? false : true ; } set { SetValue("new_emailreadstatus", value); } }
    }

    public enum EmailSignatureTypes
    {
        Arij,
        ArijArabic,
        Romak,
        Miracle,
        Null,
    }
}
