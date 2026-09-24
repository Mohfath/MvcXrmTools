using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MvcTeam.Utilities.Services
{
    public class HtmlTableStyle
    {
        public string BorderColor { get; set; }
        public string HeaderBackgroundColor { get; set; }
        public string HeaderFontColor { get; set; }
        public bool BoldHeader { get; set; }
        public string RowBackgroundColor { get; set; }
        public string RowFontColor { get; set; }
        public string AlternatingRowBackgroundColor { get; set; }
        public string AlternatingRowFontColor { get; set; }
    }

    public class QueryRecords
    {
        public string EntityName { get; set; }
        public List<Guid> Ids { get; } = new List<Guid>();
    }

    //The rows of a view or FetchXml query as text. Each cell has the text a user would see (Rows) and a
    //machine-readable value (RawValues): lookup id, option set number, ISO UTC date, '.' decimals.
    //Based on WorkflowQueryBase and QueryGetResults from Kaskela.WorkflowElements.
    public class QueryResultTable
    {
        public List<string> Headers { get; } = new List<string>();
        public List<string[]> Rows { get; } = new List<string[]>();
        public List<string[]> RawValues { get; } = new List<string[]>();

        //Priority as in the original: personal view, then system view, then the FetchXml text.
        //The layout of a view decides the columns; without one the attributes of the FetchXml are used.
        //A filter "primary key is not null" on a link to the current record's entity is narrowed to the current record.
        public static QueryResultTable Load(IOrganizationService service, EntityReference savedQuery, EntityReference userQuery,
            string fetchXml, string currentEntityName, Guid currentEntityId)
        {
            string layoutXml;
            var fetch = Prepare(service, savedQuery, userQuery, fetchXml, currentEntityName, currentEntityId, out layoutXml);

            var columns = BuildColumns(service, fetch, layoutXml);
            if (columns.Count == 0)
                throw new InvalidPluginExecutionException("The query returns no columns. List the attributes to return in the view or FetchXml (all-attributes isn't supported).");

            var table = new QueryResultTable();
            table.Headers.AddRange(columns.Select(c => c.Header));
            foreach (var entity in RunAllPages(service, fetch))
            {
                table.AddRow(entity, columns);
            }
            return table;
        }

        //Same query rules as Load, but returns only the entity name and the ids of the matching records
        public static QueryRecords LoadRecordIds(IOrganizationService service, EntityReference savedQuery, EntityReference userQuery,
            string fetchXml, string currentEntityName, Guid currentEntityId)
        {
            string layoutXml;
            var fetch = Prepare(service, savedQuery, userQuery, fetchXml, currentEntityName, currentEntityId, out layoutXml);

            var records = new QueryRecords { EntityName = (string)fetch.Root.Element("entity").Attribute("name") };
            foreach (var entity in RunAllPages(service, fetch))
            {
                //Aggregate and grouped queries have no record behind a row
                if (entity.Id == Guid.Empty)
                    throw new InvalidPluginExecutionException("The query must return records, not aggregated values.");
                records.Ids.Add(entity.Id);
            }
            return records;
        }

        private static XDocument Prepare(IOrganizationService service, EntityReference savedQuery, EntityReference userQuery,
            string fetchXml, string currentEntityName, Guid currentEntityId, out string layoutXml)
        {
            layoutXml = "";
            if (userQuery != null)
            {
                ReadView(service, "userquery", userQuery.Id, out fetchXml, out layoutXml);
            }
            else if (savedQuery != null)
            {
                ReadView(service, "savedquery", savedQuery.Id, out fetchXml, out layoutXml);
            }

            var fetch = ParseFetch(fetchXml);
            LimitToCurrentRecord(service, fetch, currentEntityName, currentEntityId);
            return fetch;
        }

        public string ToHtml(HtmlTableStyle style, bool includeHeader)
        {
            var html = new StringBuilder();
            html.Append($"<table style=\"border: 1px solid {style.BorderColor}; border-collapse:collapse;\">");
            if (includeHeader)
            {
                var fontWeight = style.BoldHeader ? "bold" : "normal";
                html.Append("<tr>");
                foreach (var header in Headers)
                {
                    html.Append($"<td style=\"border: 1px solid {style.BorderColor}; padding: 6px; background-color: {style.HeaderBackgroundColor}; color: {style.HeaderFontColor}; font-weight: {fontWeight}\">");
                    html.Append(WebUtility.HtmlEncode(header));
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
            for (var rowNumber = 0; rowNumber < Rows.Count; rowNumber++)
            {
                var even = rowNumber % 2 == 0;
                html.Append($"<tr style=\"background-color: {(even ? style.RowBackgroundColor : style.AlternatingRowBackgroundColor)}; color: {(even ? style.RowFontColor : style.AlternatingRowFontColor)}\">");
                foreach (var cell in Rows[rowNumber])
                {
                    html.Append($"<td style=\"border: 1px solid {style.BorderColor}; padding: 6px; \">");
                    html.Append(WebUtility.HtmlEncode(cell));
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }

        public string ToCsv(bool includeHeader)
        {
            var lines = new List<string>();
            if (includeHeader) lines.Add(string.Join(",", Headers.Select(CsvField)));
            lines.AddRange(Rows.Select(row => string.Join(",", row.Select(CsvField))));
            return string.Join(Environment.NewLine, lines);
        }

        //The first column of every row
        public List<string> ToList(bool includeEmptyItems)
        {
            var items = Rows.Select(row => row[0]);
            if (!includeEmptyItems) items = items.Where(item => !string.IsNullOrEmpty(item));
            return items.ToList();
        }

        private static string CsvField(string value)
        {
            if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0) return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private class Column
        {
            public string Entity;
            public string EntityAlias;
            public string Attribute;
            public string AttributeAlias;
            public string Header;

            //The name the value has in the query result
            public string ResultKey
            {
                get
                {
                    if (!string.IsNullOrEmpty(AttributeAlias)) return AttributeAlias;
                    return string.IsNullOrEmpty(EntityAlias) ? Attribute : EntityAlias + "." + Attribute;
                }
            }
        }

        private static void ReadView(IOrganizationService service, string viewEntity, Guid viewId, out string fetchXml, out string layoutXml)
        {
            var view = service.Retrieve(viewEntity, viewId, new ColumnSet("fetchxml", "layoutxml"));
            fetchXml = view.GetAttributeValue<string>("fetchxml") ?? "";
            layoutXml = view.GetAttributeValue<string>("layoutxml") ?? "";
        }

        private static XDocument ParseFetch(string fetchXml)
        {
            if (string.IsNullOrWhiteSpace(fetchXml))
                throw new InvalidPluginExecutionException("The query has no FetchXml.");

            XDocument fetch;
            try
            {
                fetch = XDocument.Parse(fetchXml);
            }
            catch (XmlException ex)
            {
                throw new InvalidPluginExecutionException($"The FetchXml is not valid: {ex.Message}", ex);
            }
            if (fetch.Root.Name != "fetch" || fetch.Root.Element("entity") == null)
                throw new InvalidPluginExecutionException("The FetchXml must be a <fetch> with an <entity>.");
            return fetch;
        }

        private static void LimitToCurrentRecord(IOrganizationService service, XDocument fetch, string entityName, Guid entityId)
        {
            if (string.IsNullOrEmpty(entityName)) return;

            var primaryKey = ((RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest { LogicalName = entityName }))
                .EntityMetadata.PrimaryIdAttribute;

            foreach (var condition in fetch.Descendants("condition"))
            {
                if ((string)condition.Attribute("attribute") != primaryKey || (string)condition.Attribute("operator") != "not-null") continue;

                var filter = condition.Parent;
                var link = filter?.Parent;
                if (filter == null || filter.Name != "filter") continue;
                if (link == null || link.Name != "link-entity" || (string)link.Attribute("name") != entityName) continue;

                condition.SetAttributeValue("operator", "eq");
                condition.SetAttributeValue("value", entityId.ToString());
            }
        }

        private static List<Column> BuildColumns(IOrganizationService service, XDocument fetch, string layoutXml)
        {
            var entityElement = fetch.Root.Element("entity");
            var primaryEntity = (string)entityElement.Attribute("name");

            var fetchColumns = new List<Column>();
            CollectAttributes(entityElement, primaryEntity, null, fetchColumns);

            var columns = fetchColumns;
            if (!string.IsNullOrWhiteSpace(layoutXml))
            {
                var layoutColumns = ReadLayout(layoutXml, primaryEntity, fetchColumns);
                if (layoutColumns.Count > 0) columns = layoutColumns;
            }

            SetHeaders(service, columns);
            return columns;
        }

        //Only attributes listed directly under <entity> or a <link-entity> count, not those inside filters
        private static void CollectAttributes(XElement parent, string entityName, string entityAlias, List<Column> columns)
        {
            foreach (var child in parent.Elements())
            {
                if (child.Name == "attribute")
                {
                    columns.Add(new Column
                    {
                        Entity = entityName,
                        EntityAlias = entityAlias,
                        Attribute = (string)child.Attribute("name"),
                        AttributeAlias = (string)child.Attribute("alias")
                    });
                }
                else if (child.Name == "link-entity")
                {
                    var linkName = (string)child.Attribute("name");
                    CollectAttributes(child, linkName, (string)child.Attribute("alias") ?? linkName, columns);
                }
            }
        }

        //View columns: "attribute" of the primary entity, or "alias.attribute" of a linked entity
        private static List<Column> ReadLayout(string layoutXml, string primaryEntity, List<Column> fetchColumns)
        {
            XDocument layout;
            try
            {
                layout = XDocument.Parse(layoutXml);
            }
            catch (XmlException ex)
            {
                throw new InvalidPluginExecutionException($"The view layout is not valid: {ex.Message}", ex);
            }

            var columns = new List<Column>();
            var row = layout.Root?.Element("row");
            if (row == null) return columns;

            foreach (var cell in row.Elements())
            {
                var name = (string)cell.Attribute("name");
                if (string.IsNullOrEmpty(name)) continue;

                var dot = name.IndexOf('.');
                if (dot < 0)
                {
                    columns.Add(new Column { Entity = primaryEntity, Attribute = name });
                }
                else
                {
                    var alias = name.Substring(0, dot);
                    columns.Add(new Column
                    {
                        Entity = fetchColumns.FirstOrDefault(f => f.EntityAlias == alias)?.Entity,
                        EntityAlias = alias,
                        Attribute = name.Substring(dot + 1)
                    });
                }
            }
            return columns;
        }

        //Header = the attribute's own alias, else its display name in the user's language, else its logical name
        private static void SetHeaders(IOrganizationService service, List<Column> columns)
        {
            foreach (var column in columns.Where(c => !string.IsNullOrEmpty(c.AttributeAlias)))
            {
                column.Header = column.AttributeAlias;
            }

            foreach (var group in columns.Where(c => string.IsNullOrEmpty(c.AttributeAlias) && c.Entity != null).GroupBy(c => c.Entity))
            {
                var metadata = ((RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.Attributes,
                    LogicalName = group.Key
                })).EntityMetadata;

                foreach (var column in group)
                {
                    var attribute = metadata.Attributes.FirstOrDefault(a => a.LogicalName == column.Attribute);
                    column.Header = attribute?.DisplayName?.UserLocalizedLabel?.Label;
                }
            }

            foreach (var column in columns.Where(c => string.IsNullOrEmpty(c.Header)))
            {
                column.Header = column.Attribute;
            }
        }

        //A view can match more than one page of records; the first page is asked as written
        private static List<Entity> RunAllPages(IOrganizationService service, XDocument fetch)
        {
            var entities = new List<Entity>();
            var page = 1;

            while (true)
            {
                EntityCollection result;
                try
                {
                    result = service.RetrieveMultiple(new FetchExpression(fetch.ToString(SaveOptions.DisableFormatting)));
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"The query failed: {ex.Message}", ex);
                }

                entities.AddRange(result.Entities);
                if (!result.MoreRecords) break;

                page++;
                fetch.Root.SetAttributeValue("page", page);
                fetch.Root.SetAttributeValue("paging-cookie", result.PagingCookie);
            }
            return entities;
        }

        private void AddRow(Entity entity, List<Column> columns)
        {
            var display = new string[columns.Count];
            var raw = new string[columns.Count];

            for (var i = 0; i < columns.Count; i++)
            {
                var key = columns[i].ResultKey;
                display[i] = "";
                raw[i] = "";
                if (!entity.Contains(key)) continue;

                var value = entity[key];
                if (value is AliasedValue aliased) value = aliased.Value;

                switch (value)
                {
                    case null:
                        break;
                    case EntityReference reference:
                        display[i] = reference.Name ?? "";
                        raw[i] = reference.Id.ToString();
                        break;
                    case string text:
                        display[i] = text;
                        raw[i] = text;
                        break;
                    case EntityCollection parties:
                        //Activity party lists (to, cc, ...) show the names of the parties
                        display[i] = string.Join("; ", parties.Entities.Select(PartyName).Where(name => !string.IsNullOrEmpty(name)));
                        break;
                    default:
                        raw[i] = RawText(value);
                        display[i] = entity.FormattedValues.Contains(key) ? entity.FormattedValues[key] : raw[i];
                        break;
                }
            }

            Rows.Add(display);
            RawValues.Add(raw);
        }

        private static string PartyName(Entity party)
        {
            var partyId = party.GetAttributeValue<EntityReference>("partyid");
            if (partyId != null && !string.IsNullOrEmpty(partyId.Name)) return partyId.Name;
            return party.FormattedValues.Contains("partyid") ? party.FormattedValues["partyid"] : null;
        }

        //Option sets give their number, dates ISO in UTC, numbers with '.' decimals
        private static string RawText(object value)
        {
            switch (value)
            {
                case OptionSetValue option: return option.Value.ToString(CultureInfo.InvariantCulture);
                case Money money: return money.Value.ToString(CultureInfo.InvariantCulture);
                case bool flag: return flag ? "true" : "false";
                case DateTime date:
                    var utc = date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : DateTime.SpecifyKind(date, DateTimeKind.Utc);
                    return utc.ToString(IsoDateFormat, CultureInfo.InvariantCulture);
                case Guid id: return id.ToString();
                case IFormattable formattable: return formattable.ToString(null, CultureInfo.InvariantCulture);
                default: return value.ToString();
            }
        }

        public const string IsoDateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";
    }
}
