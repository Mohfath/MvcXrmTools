using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System;
using System.Activities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MvcTeam.Utilities.Workflows
{
    //Runs a view or FetchXml and returns the rows as an HTML table, CSV, a list of the first column,
    //and the first row's values as typed single values.
    //Based on QueryGetResults from Kaskela.WorkflowElements.
    public class QueryGetResults : CodeActivity
    {
        private static readonly Regex ColorCode = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");

        [Input("Pick a System View to Use")]
        [ReferenceTarget("savedquery")]
        public InArgument<EntityReference> SavedQuery { get; set; }

        [Input("or pick a Personal View to Use")]
        [ReferenceTarget("userquery")]
        public InArgument<EntityReference> UserQuery { get; set; }

        [Input("or enter FetchXML to Use")]
        public InArgument<string> FetchXml { get; set; }

        [Input("Table - Include Header")]
        [Default("True")]
        public InArgument<bool> IncludeHeader { get; set; }

        [RequiredArgument]
        [Input("Table - Border Color")]
        [Default("#000000")]
        public InArgument<string> Table_BorderColor { get; set; }

        [RequiredArgument]
        [Input("Table - Header Background Color")]
        [Default("#6495ED")]
        public InArgument<string> Header_BackgroundColor { get; set; }

        [RequiredArgument]
        [Input("Table - Header Font Color")]
        [Default("#FFFFFF")]
        public InArgument<string> Header_FontColor { get; set; }

        [RequiredArgument]
        [Input("Table - Header Bold Font?")]
        [Default("True")]
        public InArgument<bool> Header_BoldFont { get; set; }

        [RequiredArgument]
        [Input("Table - Row Background Color")]
        [Default("#FFFFFF")]
        public InArgument<string> Row_BackgroundColor { get; set; }

        [RequiredArgument]
        [Input("Table - Row Font Color")]
        [Default("#000000")]
        public InArgument<string> Row_FontColor { get; set; }

        [RequiredArgument]
        [Input("Table - Alternating Row Background Color")]
        [Default("#F0F8FF")]
        public InArgument<string> AlternatingRow_BackgroundColor { get; set; }

        [RequiredArgument]
        [Input("Table - Alternating Row Font Color")]
        [Default("#000000")]
        public InArgument<string> AlternatingRow_FontColor { get; set; }

        [Input("List - Item separator")]
        [Default(", ")]
        public InArgument<string> ListSeparator { get; set; }

        [Input("List - Include empty items?")]
        [Default("False")]
        public InArgument<bool> IncludeEmptyItem { get; set; }

        [Output("# of Results")]
        public OutArgument<int> NumberOfResults { get; set; }

        [Output("Table - Query Results (HTML)")]
        public OutArgument<string> HtmlQueryResults { get; set; }

        [Output("Table - Query Results (CSV)")]
        public OutArgument<string> CsvQueryResults { get; set; }

        [Output("List - Query Results as a List")]
        public OutArgument<string> ListResults { get; set; }

        [Output("Single Value - Result as Whole Number")]
        public OutArgument<int> QueryResult_WholeNumber { get; set; }

        [Output("Single Value - Result as DateTime")]
        public OutArgument<DateTime> QueryResult_DateTime { get; set; }

        [Output("Single Value - Result as Decimal")]
        public OutArgument<decimal> QueryResult_Decimal { get; set; }

        [Output("Single Value - Result as Double")]
        public OutArgument<double> QueryResult_Double { get; set; }

        [Output("Single Value - Result as Money")]
        public OutArgument<Money> QueryResult_Money { get; set; }

        [Output("Single Value - Result as ID")]
        public OutArgument<string> QueryResult_Guid { get; set; }

        [Output("Single Value - Result as Text")]
        public OutArgument<string> QueryResult_Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var style = new HtmlTableStyle
            {
                BorderColor = ValidColor(Table_BorderColor.Get(context), "Table - Border Color"),
                HeaderBackgroundColor = ValidColor(Header_BackgroundColor.Get(context), "Header - Background Color"),
                HeaderFontColor = ValidColor(Header_FontColor.Get(context), "Header - Font Color"),
                BoldHeader = Header_BoldFont.Get(context),
                RowBackgroundColor = ValidColor(Row_BackgroundColor.Get(context), "Row - Background Color"),
                RowFontColor = ValidColor(Row_FontColor.Get(context), "Row - Font Color"),
                AlternatingRowBackgroundColor = ValidColor(AlternatingRow_BackgroundColor.Get(context), "Alternating Row - Background Color"),
                AlternatingRowFontColor = ValidColor(AlternatingRow_FontColor.Get(context), "Alternating Row - Font Color")
            };

            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var savedQuery = SavedQuery.Get(context);
            var userQuery = UserQuery.Get(context);
            var fetchXml = FetchXml.Get(context);
            if (savedQuery == null && userQuery == null && string.IsNullOrWhiteSpace(fetchXml))
                throw new InvalidPluginExecutionException("You need to pick a System View, a Personal View, or specify FetchXML for the query.");

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //Runs as the workflow's user, so it only finds what that user can read
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var table = QueryResultTable.Load(service, savedQuery, userQuery, fetchXml,
                workflowContext.PrimaryEntityName, workflowContext.PrimaryEntityId);

            tracingService.Trace("Query returned {0} row(s) and {1} column(s)", table.Rows.Count, table.Headers.Count);

            NumberOfResults.Set(context, table.Rows.Count);
            CsvQueryResults.Set(context, table.ToCsv(IncludeHeader.Get(context)));
            HtmlQueryResults.Set(context, table.ToHtml(style, IncludeHeader.Get(context)));
            ListResults.Set(context, string.Join(ListSeparator.Get(context), table.ToList(IncludeEmptyItem.Get(context))));
            SetSingleValueResults(table, context);
        }

        private static string ValidColor(string value, string fieldName)
        {
            if (value == null || !ColorCode.IsMatch(value))
                throw new InvalidPluginExecutionException($"Hex code invalid for '{fieldName}' - value must be formatted as '#xxxxxx'");
            return value;
        }

        //The first column of the first row, as every type it can be read as
        private void SetSingleValueResults(QueryResultTable table, CodeActivityContext context)
        {
            if (table.Rows.Count == 0) return;

            QueryResult_Text.Set(context, table.Rows[0][0]);
            var value = table.RawValues[0][0];

            Guid id;
            if (Guid.TryParse(value, out id)) QueryResult_Guid.Set(context, value);

            DateTime date;
            if (DateTime.TryParseExact(value, QueryResultTable.IsoDateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out date))
                QueryResult_DateTime.Set(context, date);

            decimal decimalValue;
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                QueryResult_Decimal.Set(context, decimalValue);
                QueryResult_Money.Set(context, new Money(decimalValue));
            }

            double doubleValue;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue))
                QueryResult_Double.Set(context, doubleValue);

            int intValue;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
                QueryResult_WholeNumber.Set(context, intValue);
        }
    }
}
