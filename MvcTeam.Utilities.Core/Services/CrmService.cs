using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Xml.Linq;

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

        public bool IsUserMemberOfTeam(Guid userId, Guid teamId)
        {
            var query = new QueryExpression("teammembership")
            {
                ColumnSet = new ColumnSet("teammembershipid"),
                TopCount = 1
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            query.Criteria.AddCondition("teamid", ConditionOperator.Equal, teamId);

            return _orgService.RetrieveMultiple(query).Entities.Count != 0;
        }

        //Only roles assigned directly to the user count, not roles the user gets through a team
        public bool UserHasRole(Guid userId, Guid roleId)
        {
            var rootRoleId = GetRootRoleId(roleId);

            var query = new QueryExpression("systemuserroles")
            {
                ColumnSet = new ColumnSet("systemuserroleid"),
                TopCount = 1
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            var roleLink = query.AddLink("role", "roleid", "roleid");
            roleLink.LinkCriteria.AddCondition("parentrootroleid", ConditionOperator.Equal, rootRoleId);

            return _orgService.RetrieveMultiple(query).Entities.Count != 0;
        }

        //True if any team the user belongs to has the role; roles assigned directly to the user don't count
        public bool UserTeamsHaveRole(Guid userId, Guid roleId)
        {
            var rootRoleId = GetRootRoleId(roleId);

            var query = new QueryExpression("teammembership")
            {
                ColumnSet = new ColumnSet("teammembershipid"),
                TopCount = 1
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);
            var teamRoleLink = query.AddLink("teamroles", "teamid", "teamid");
            var roleLink = teamRoleLink.AddLink("role", "roleid", "roleid");
            roleLink.LinkCriteria.AddCondition("parentrootroleid", ConditionOperator.Equal, rootRoleId);

            return _orgService.RetrieveMultiple(query).Entities.Count != 0;
        }

        //Each business unit has its own copy of a role; all copies point to the same root role,
        //so role checks compare root roles, whichever copy was picked in the workflow
        private Guid GetRootRoleId(Guid roleId)
        {
            var role = _orgService.Retrieve("role", roleId, new ColumnSet("parentrootroleid"));
            return role.GetAttributeValue<EntityReference>("parentrootroleid")?.Id ?? roleId;
        }

        //Turns a "Record Url (Dynamic)" value into a record reference. Old-style URLs (etc=) need a metadata lookup.
        public EntityReference GetRecordFromUrl(string recordUrl)
        {
            DynamicUrlParser parser;
            try
            {
                parser = new DynamicUrlParser(recordUrl);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Record Reference '{recordUrl}' is not a valid record URL.", ex);
            }
            if (parser.Id == Guid.Empty)
                throw new InvalidPluginExecutionException($"Record Reference '{recordUrl}' has no record id.");

            var logicalName = parser.GetEntityLogicalName(_orgService);
            if (string.IsNullOrEmpty(logicalName))
                throw new InvalidPluginExecutionException($"Record Reference '{recordUrl}' has an unknown entity type.");

            return new EntityReference(logicalName, parser.Id);
        }

        //Converts with the exchange rates kept in CRM's currency records (ISO codes such as IRR, USD, EUR).
        //A currency's rate is how many units of it equal 1 unit of the base currency.
        public decimal ConvertCurrency(decimal amount, string fromIsoCode, string toIsoCode)
        {
            var from = fromIsoCode.Trim().ToUpperInvariant();
            var to = toIsoCode.Trim().ToUpperInvariant();
            if (from == to) return amount;

            return amount * GetExchangeRate(to) / GetExchangeRate(from);
        }

        private decimal GetExchangeRate(string isoCode)
        {
            var query = new QueryExpression("transactioncurrency")
            {
                ColumnSet = new ColumnSet("exchangerate"),
                TopCount = 1
            };
            query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, isoCode);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var currency = _orgService.RetrieveMultiple(query).Entities.FirstOrDefault();
            if (currency == null)
                throw new InvalidPluginExecutionException($"Currency '{isoCode}' is not set up as an active currency in CRM.");

            var rate = currency.GetAttributeValue<decimal?>("exchangerate");
            if (rate == null || rate <= 0)
                throw new InvalidPluginExecutionException($"Currency '{isoCode}' has no valid exchange rate in CRM.");
            return rate.Value;
        }

        //Returns the first record whose fields equal the given text values (an empty value means the field is empty),
        //with only the requested columns; null if nothing matches. FetchXml lets CRM convert the text to each field's type.
        public Entity FindFirst(string entityName, IEnumerable<string> columns, IDictionary<string, string> filters)
        {
            var fetch = new XElement("fetch", new XAttribute("top", 1),
                new XElement("entity", new XAttribute("name", entityName),
                    columns.Select(c => new XElement("attribute", new XAttribute("name", c))),
                    new XElement("filter", new XAttribute("type", "and"),
                        filters.Select(f => string.IsNullOrEmpty(f.Value)
                            ? new XElement("condition", new XAttribute("attribute", f.Key), new XAttribute("operator", "null"))
                            : new XElement("condition", new XAttribute("attribute", f.Key), new XAttribute("operator", "eq"), new XAttribute("value", f.Value))))));

            return _orgService.RetrieveMultiple(new FetchExpression(fetch.ToString())).Entities.FirstOrDefault();
        }

        //Runs the FetchXml page by page and returns the values of its "holiday" column (a plain or aliased date)
        public List<DateTime> GetHolidayDates(string fetchXml)
        {
            XDocument fetch;
            try
            {
                fetch = XDocument.Parse(fetchXml);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Holidays Query is not valid FetchXml: {ex.Message}", ex);
            }

            var holidays = new List<DateTime>();
            var page = 1;
            string pagingCookie = null;

            while (true)
            {
                fetch.Root.SetAttributeValue("page", page);
                fetch.Root.SetAttributeValue("paging-cookie", pagingCookie);

                EntityCollection result;
                try
                {
                    result = _orgService.RetrieveMultiple(new FetchExpression(fetch.ToString()));
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"Holidays Query failed: {ex.Message}", ex);
                }

                foreach (var row in result.Entities)
                {
                    if (!row.Contains("holiday")) continue;
                    var value = row["holiday"] is AliasedValue aliased ? aliased.Value : row["holiday"];
                    if (value is DateTime date) holidays.Add(date);
                }

                if (!result.MoreRecords) break;
                page++;
                pagingCookie = result.PagingCookie;
            }

            return holidays;
        }

        //Recalculates every published rollup field of the record; returns the fields it recalculated
        public List<string> RecalculateAllRollups(EntityReference target)
        {
            var response = (RetrieveEntityResponse)_orgService.Execute(new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = target.LogicalName,
                //Unpublished rollups can't be calculated yet
                RetrieveAsIfPublished = false
            });

            //SourceType 2 = rollup. A money rollup also has a base-currency copy (CalculationOf is set)
            //that CRM calculates together with it, so it is skipped
            var rollupFields = response.EntityMetadata.Attributes
                .Where(a => a.SourceType == 2 && !(a is MoneyAttributeMetadata money && money.CalculationOf != null))
                .Select(a => a.LogicalName)
                .ToList();

            foreach (var field in rollupFields)
            {
                _tracingService?.Trace("Recalculating rollup field {0}", field);
                try
                {
                    _orgService.Execute(new CalculateRollupFieldRequest { Target = target, FieldName = field });
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"Could not recalculate rollup field '{field}': {ex.Message}", ex);
                }
            }
            return rollupFields;
        }

        //Starts the workflow on every record, one after the other, and stops at the first failure.
        //Returns how many were started. The workflow must be for the same entity as the records.
        public int RunWorkflowOnRecords(Guid workflowId, QueryRecords records)
        {
            var workflow = _orgService.Retrieve("workflow", workflowId, new ColumnSet("primaryentity"));
            var workflowEntity = workflow.GetAttributeValue<string>("primaryentity");
            if (!string.Equals(workflowEntity, records.EntityName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidPluginExecutionException($"The workflow's entity ({workflowEntity}) does not match the query's entity ({records.EntityName}).");

            var started = 0;
            foreach (var id in records.Ids)
            {
                try
                {
                    _orgService.Execute(new ExecuteWorkflowRequest { EntityId = id, WorkflowId = workflowId });
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"Could not start the workflow on record {id} ({started} started before it): {ex.Message}", ex);
                }
                started++;
            }
            return started;
        }

        public SaleOrder GetSaleOrderById(Guid saleOrderId)
        {
            var order = new SaleOrder(_orgService.Retrieve("salesorder", saleOrderId, new ColumnSet(true)));
            order.SaleOrderItems = GetSaleOrderItemsForSaleOrder(saleOrderId);
            return order;
        }

        private List<SalesOrderItem> GetSaleOrderItemsForSaleOrder(Guid saleOrderId)
        {
            var output = new List<SalesOrderItem>();
            QueryExpression query = new QueryExpression
            {
                EntityName = "salesorderdetail",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression { Conditions = { new ConditionExpression { AttributeName = "salesorderid", Operator = ConditionOperator.Equal, Values = { saleOrderId } } } }
            };
            var result = _orgService.RetrieveMultiple(query);
            foreach (var item in result.Entities)
            {
                output.Add(new SalesOrderItem(item));
            }
            return output;
        }

        internal Invoice GetInvoiceById(Guid id)
        {
            var invoice = new Invoice(_orgService.Retrieve("invoice", id, new ColumnSet(true)));
            invoice.InvoiceItems = GetInvoiceItemsForInvoice(id).ToList();
            return invoice;
        }

        private IEnumerable<InvoiceItem> GetInvoiceItemsForInvoice(Guid invoiceId)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "invoicedetail",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression { Conditions = { new ConditionExpression { AttributeName = "invoiceid", Operator = ConditionOperator.Equal, Values = { invoiceId } } } }
            };
            var result = _orgService.RetrieveMultiple(query);
            foreach (var item in result.Entities)
            {
                yield return new InvoiceItem(item);
            }
        }

        public void DeleteItem(EntityObject item)
        {
            _orgService.Delete(item.Entity.LogicalName, item.Id);
        }

        internal IEnumerable<Invoice> GetInvoicesForSaleOrder(Guid id)
        {
            var query = new QueryExpression("invoice");
            query.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, id);
            query.ColumnSet = new ColumnSet(true);

            foreach (var item in _orgService.RetrieveMultiple(query).Entities)
            {
                var tempInvoice = new Invoice(item);
                tempInvoice.InvoiceItems = GetInvoiceItemsForInvoice(tempInvoice.Id).ToList();
                yield return tempInvoice;
            }
        }

        public void UpdateEntity(EntityObject item)
        {
            _orgService.Update(item.Entity);
        }

        //Updates the user's personal settings (paging limit, advanced find, time zone, help/UI language,
        //default calendar view, send-as). A value of 0 means "leave it unchanged" for paging limit, time zone and
        //help/UI language; advanced find mode is written only when it is 1 or 2. Default calendar view (0 = day)
        //and send-as are always written. The usersettings record is created on first use, so a missing record is created.
        //Based on SetUserSettings from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko).
        public void SetUserSettings(EntityReference user, int pagingLimit, int advancedFindStartupMode, int timeZoneCode,
            int helpLanguageId, int uiLanguageId, int defaultCalendarView, bool isSendAsAllowed)
        {
            var query = new QueryExpression("usersettings")
            {
                ColumnSet = new ColumnSet(true),
                TopCount = 1
            };
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, user.Id);
            var settings = _orgService.RetrieveMultiple(query).Entities.FirstOrDefault();
            if (settings == null)
            {
                settings = new Entity("usersettings");
                settings["systemuserid"] = user;
            }

            if (pagingLimit != 0) settings["paginglimit"] = pagingLimit;
            if (advancedFindStartupMode == 1 || advancedFindStartupMode == 2)
                settings["advancedfindstartupmode"] = advancedFindStartupMode;
            if (timeZoneCode != 0) settings["timezonecode"] = timeZoneCode;
            if (helpLanguageId != 0) settings["helplanguageid"] = helpLanguageId;
            if (uiLanguageId != 0) settings["uilanguageid"] = uiLanguageId;
            if (defaultCalendarView == 0 || defaultCalendarView == 1 || defaultCalendarView == 2)
                settings["defaultcalendarview"] = defaultCalendarView;
            settings["issendasallowed"] = isSendAsAllowed;

            if (settings.Id == Guid.Empty)
                _orgService.Create(settings);
            else
                _orgService.Update(settings);
        }

        internal void ClearInvoiceItems(Guid id)
        {
            var items = GetInvoiceItemsForInvoice(id);

            foreach (var item in items)
            {
                _orgService.Delete("invoicedetail", item.Id);
            }
        }

        internal void AddInvoiceItemsToInvoice(Guid id, IEnumerable<InvoiceItem> computedInvoiceItems)
        {
            foreach (var item in computedInvoiceItems.ToList())
            {
                item.InvoiceId = id;
                _orgService.Create(item.Entity);
            }
        }
    }
}

