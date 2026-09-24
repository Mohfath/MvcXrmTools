using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace MvcTeam.Utilities.Models
{
    public class CrmEmailSignature : EntityObject
    {
        public CrmEmailSignature(Entity entity) : base(entity)
        {
        }


        public EmailSignatureTypes? SignatureType { get
            {
                if (Title.ToLower().Contains("romak")) return EmailSignatureTypes.Romak;
                if (Title.ToLower().Contains("arabi")) return EmailSignatureTypes.ArijArabic;
                if (Title.ToLower().Contains("arij")) return EmailSignatureTypes.Arij;
                if (Title.ToLower().Contains("miracle")) return EmailSignatureTypes.Miracle;

                return null;
            } }

        public string Title { get { return (string)Entity["title"]; } }
        public string PresentationXml { get { return (string)Entity["presentationxml"]; } }
    }


}
