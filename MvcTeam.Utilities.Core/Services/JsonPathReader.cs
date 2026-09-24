using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MvcTeam.Utilities.Services
{
    //Reads one value out of a JSON text using a simple path, with .NET's built-in JSON reader
    //(no extra DLL, so it runs in the CRM sandbox).
    //Path forms: "a.b", "a[0]", "a['name with space']", "$" (whole document); combine freely, e.g. "$.values[0].['Response Date']".
    //Strings come back unquoted, numbers exactly as written, true/false, null as empty, objects/arrays as compact JSON.
    //A path that doesn't exist gives an empty result.
    public static class JsonPathReader
    {
        public static string Read(string json, string path)
        {
            XElement node;
            try
            {
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
                {
                    node = XElement.Load(reader);
                }
            }
            catch (Exception ex) when (ex is XmlException || ex is SerializationException)
            {
                throw new InvalidPluginExecutionException($"JSON is not valid: {ex.Message}", ex);
            }

            foreach (var step in ParsePath(path ?? ""))
            {
                node = step is int index ? Item(node, index) : Member(node, (string)step);
                if (node == null) return "";
            }
            return ToText(node);
        }

        private static XElement Member(XElement node, string name)
        {
            if ((string)node.Attribute("type") != "object") return null;
            //Keys that aren't valid XML names are stored as <a:item item="key">
            return node.Elements().FirstOrDefault(e =>
                (e.Name.Namespace == XNamespace.None && e.Name.LocalName == name) ||
                (e.Name.LocalName == "item" && (string)e.Attribute("item") == name));
        }

        private static XElement Item(XElement node, int index)
        {
            if ((string)node.Attribute("type") != "array") return null;
            return node.Elements().ElementAtOrDefault(index);
        }

        private static string ToText(XElement node)
        {
            switch ((string)node.Attribute("type"))
            {
                case "string":
                case "number":
                case "boolean":
                    return node.Value;
                case "null":
                    return "";
                default:
                    return ToJson(node);
            }
        }

        private static string ToJson(XElement node)
        {
            var root = new XElement("root", node.Attribute("type"), node.Nodes());
            using (var stream = new MemoryStream())
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false))
                {
                    root.WriteTo(writer);
                    writer.Flush();
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        //Splits a path into member names (string) and array indexes (int)
        private static List<object> ParsePath(string path)
        {
            var steps = new List<object>();
            var p = path.Trim();
            var i = p.StartsWith("$") ? 1 : 0;

            while (i < p.Length)
            {
                if (p[i] == '.')
                {
                    i++;
                    if (i < p.Length && p[i] == '[') continue;   // ".['name']" form
                    steps.Add(ReadName(p, ref i));
                }
                else if (p[i] == '[')
                {
                    var close = p.IndexOf(']', i);
                    if (close < 0) throw BadPath(path, "missing ']'");
                    var inside = p.Substring(i + 1, close - i - 1).Trim();
                    if (inside.Length >= 2 && (inside[0] == '\'' || inside[0] == '"') && inside[inside.Length - 1] == inside[0])
                        steps.Add(inside.Substring(1, inside.Length - 2));
                    else if (int.TryParse(inside, NumberStyles.None, CultureInfo.InvariantCulture, out var index))
                        steps.Add(index);
                    else
                        throw BadPath(path, $"'[{inside}]' must be a number or a quoted name");
                    i = close + 1;
                }
                else if (steps.Count == 0)
                {
                    steps.Add(ReadName(p, ref i));   // path starting with a name, e.g. "a.b"
                }
                else
                {
                    throw BadPath(path, $"unexpected '{p[i]}'");
                }
            }
            return steps;
        }

        private static string ReadName(string p, ref int i)
        {
            var start = i;
            while (i < p.Length && p[i] != '.' && p[i] != '[') i++;
            var name = p.Substring(start, i - start);
            if (name.Length == 0) throw BadPath(p, "empty name");
            return name;
        }

        private static InvalidPluginExecutionException BadPath(string path, string reason) =>
            new InvalidPluginExecutionException($"JSON Path '{path}' is not valid: {reason}.");
    }
}
