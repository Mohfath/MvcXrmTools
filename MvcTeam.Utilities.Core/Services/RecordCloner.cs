using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTeam.Utilities.Services
{
    //Based on Common.CloneRecord from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko)
    public class RecordCloner
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracingService;

        public RecordCloner(IOrganizationService service, ITracingService tracingService)
        {
            _service = service;
            _tracingService = tracingService;
        }

        //Creates a copy of the record and returns its id.
        //fieldsToIgnore: logical names separated by ';' or ','. prefix: put before the primary name.
        //overrides: values set on the copy instead of the source values (null clears a field).
        public Guid Clone(EntityReference source, string fieldsToIgnore, string prefix, IDictionary<string, object> overrides = null)
        {
            return Clone(source, GetMetadata(source.LogicalName), fieldsToIgnore, prefix, overrides);
        }

        private EntityMetadata GetMetadata(string logicalName)
        {
            return ((RetrieveEntityResponse)_service.Execute(new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = logicalName
            })).EntityMetadata;
        }

        //metadata is the entity's attribute metadata, passed in so that cloning many records of one type fetches it only once
        private Guid Clone(EntityReference source, EntityMetadata metadata, string fieldsToIgnore, string prefix, IDictionary<string, object> overrides)
        {
            var ignored = ParseFieldList(fieldsToIgnore);
            var original = _service.Retrieve(source.LogicalName, source.Id, new ColumnSet(true));
            var copy = new Entity(source.LogicalName);

            foreach (var attribute in metadata.Attributes)
            {
                var name = attribute.LogicalName;
                //Update-only fields make Create fail; status is set after the create
                if (attribute.IsValidForCreate != true || attribute.IsPrimaryId == true) continue;
                if (name == "statecode" || name == "statuscode" || ignored.Contains(name)) continue;
                if (!original.Contains(name)) continue;

                var value = original[name];
                if (attribute.AttributeType == AttributeTypeCode.PartyList)
                {
                    value = CopyParties(value as EntityCollection);
                }
                else if (name == metadata.PrimaryNameAttribute && !string.IsNullOrEmpty(prefix) && value is string text)
                {
                    value = prefix + text;
                }
                copy[name] = value;
            }

            if (overrides != null)
            {
                foreach (var pair in overrides) copy[pair.Key] = pair.Value;
            }

            _tracingService?.Trace("Creating copy of {0} {1} with {2} field(s)", source.LogicalName, source.Id, copy.Attributes.Count);
            var copyId = _service.Create(copy);

            CopyStatus(original, copyId);
            return copyId;
        }

        //Rebuilds activity parties (to, cc, attendees...). Unresolved email addresses have no partyid.
        private static EntityCollection CopyParties(EntityCollection parties)
        {
            var result = new EntityCollection();
            if (parties == null) return result;

            foreach (var party in parties.Entities)
            {
                var newParty = new Entity("activityparty");
                if (party.GetAttributeValue<EntityReference>("partyid") is EntityReference partyId)
                {
                    newParty["partyid"] = new EntityReference(partyId.LogicalName, partyId.Id);
                }
                else if (party.GetAttributeValue<string>("addressused") is string address && address != "")
                {
                    newParty["addressused"] = address;
                }
                else
                {
                    continue;
                }
                result.Entities.Add(newParty);
            }
            return result;
        }

        //New records start with the default status; give the copy the source's status if it differs
        private void CopyStatus(Entity original, Guid copyId)
        {
            var state = original.GetAttributeValue<OptionSetValue>("statecode");
            var status = original.GetAttributeValue<OptionSetValue>("statuscode");
            if (state == null || status == null) return;

            var created = _service.Retrieve(original.LogicalName, copyId, new ColumnSet("statecode", "statuscode"));
            if (created.GetAttributeValue<OptionSetValue>("statecode")?.Value == state.Value &&
                created.GetAttributeValue<OptionSetValue>("statuscode")?.Value == status.Value)
                return;

            try
            {
                _service.Update(new Entity(original.LogicalName, copyId)
                {
                    ["statecode"] = new OptionSetValue(state.Value),
                    ["statuscode"] = new OptionSetValue(status.Value)
                });
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(
                    $"The copy ({copyId}) was created, but its status could not be set to match the source: {ex.Message}", ex);
            }
        }

        //Clones every child of the parent through a one-to-many relationship and points the copies at newParent.
        //Returns the number of children cloned.
        public int CloneChildren(string relationshipName, EntityReference parent, string newParentField, EntityReference newParent,
            string oldParentField, string fieldsToIgnore, string prefix)
        {
            var relationship = ((RetrieveRelationshipResponse)_service.Execute(new RetrieveRelationshipRequest { Name = relationshipName }))
                .RelationshipMetadata as OneToManyRelationshipMetadata;
            if (relationship == null)
                throw new InvalidPluginExecutionException($"Relationship '{relationshipName}' is not a one-to-many relationship.");

            var overrides = new Dictionary<string, object> { [newParentField] = newParent };
            if (!string.IsNullOrEmpty(oldParentField) && oldParentField != newParentField)
                overrides[oldParentField] = null;

            var children = GetChildren(relationship.ReferencingEntity, relationship.ReferencingAttribute, parent.Id);
            var metadata = children.Count == 0 ? null : GetMetadata(relationship.ReferencingEntity);
            var cloned = 0;
            foreach (var child in children)
            {
                try
                {
                    Clone(child, metadata, fieldsToIgnore, prefix, overrides);
                }
                catch (Exception ex)
                {
                    //The copies already made stay; say how far it got so they can be cleaned up or the rest cloned
                    throw new InvalidPluginExecutionException($"Cloned {cloned} of {children.Count} children, then failed on child {child.Id}: {ex.Message}", ex);
                }
                cloned++;
            }
            return children.Count;
        }

        private List<EntityReference> GetChildren(string childEntity, string lookupField, Guid parentId)
        {
            var query = new QueryExpression(childEntity)
            {
                ColumnSet = new ColumnSet(false),
                PageInfo = new PagingInfo { PageNumber = 1, Count = 5000 }
            };
            query.Criteria.AddCondition(lookupField, ConditionOperator.Equal, parentId);

            var children = new List<EntityReference>();
            while (true)
            {
                var page = _service.RetrieveMultiple(query);
                children.AddRange(page.Entities.Select(e => new EntityReference(childEntity, e.Id)));
                if (!page.MoreRecords) break;
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = page.PagingCookie;
            }
            return children;
        }

        private static HashSet<string> ParseFieldList(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields)) return new HashSet<string>();
            return new HashSet<string>(fields.Split(';', ',').Select(f => f.Trim().ToLowerInvariant()).Where(f => f != ""));
        }
    }
}
