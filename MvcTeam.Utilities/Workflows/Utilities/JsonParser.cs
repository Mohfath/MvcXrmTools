using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    //Based on JsonParser from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko)
    public class JsonParser : CodeActivity
    {
        [RequiredArgument]
        [Input("JSON")]
        [Default("")]
        public InArgument<string> JSON { get; set; }

        //e.g. "parameters.Code", "values[0].Author", "values[0].['Response Date']"; empty or "$" gives the whole JSON
        [RequiredArgument]
        [Input("JSON Path")]
        [Default("")]
        public InArgument<string> JSONPath { get; set; }

        [Output("JSON Result")]
        public OutArgument<string> JSONResult { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var json = JSON.Get(context);
            if (string.IsNullOrWhiteSpace(json)) throw new InvalidPluginExecutionException("JSON is required.");
            var path = JSONPath.Get(context);

            var result = JsonPathReader.Read(json, path);

            context.GetExtension<ITracingService>().Trace("JSON Path '{0}' gave {1} character(s)", path, result.Length);
            JSONResult.Set(context, result);
        }
    }
}
