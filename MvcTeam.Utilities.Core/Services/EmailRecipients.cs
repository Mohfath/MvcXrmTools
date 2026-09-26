using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTeam.Utilities.Services
{
    //The To/CC lists of an email as the steps that add recipients need them
    public static class EmailRecipients
    {
        //The recipients already on the email ("to" or "cc"). CRM leaves the field out when the list is empty.
        public static List<Entity> Existing(Entity email, string field)
        {
            return email.GetAttributeValue<EntityCollection>(field)?.Entities.ToList() ?? new List<Entity>();
        }

        //True when the record is already a recipient. A recipient typed as a plain email address has no partyid, so it never matches.
        public static bool Contains(List<Entity> recipients, Guid id)
        {
            return recipients.Any(recipient => recipient.GetAttributeValue<EntityReference>("partyid")?.Id == id);
        }
    }
}
