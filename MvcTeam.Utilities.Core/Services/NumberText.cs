using MD.PersianDateTime;
using System.Globalization;

namespace MvcTeam.Utilities.Services
{
    //Reads numbers that people type. Persian and Arabic digits (and their decimal and thousands marks) are accepted,
    //and the server's culture never matters: "." is always the decimal separator and "," always separates thousands.
    //In Persian text "/" is also a decimal separator (12/5 is 12.5), like the Persian and Arabic decimal marks; the Arabic comma and the Persian thousands mark separate thousands.
    public static class NumberText
    {
        public static string Normalize(string text)
        {
            return ExtensionsHelper.ConvertDigitsToLatin(text.Trim())
                .Replace('\u066B', '.').Replace('/', '.')     //Persian decimal marks
                .Replace('\u066C', ',').Replace('\u060C', ',');   //Persian and Arabic thousands marks
        }

        public static bool TryParseDecimal(string text, out decimal value)
        {
            value = 0;
            return !string.IsNullOrWhiteSpace(text) && decimal.TryParse(Normalize(text), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParseInteger(string text, out int value)
        {
            value = 0;
            return !string.IsNullOrWhiteSpace(text) && int.TryParse(Normalize(text), NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParseDouble(string text, out double value)
        {
            value = 0;
            //double.TryParse also accepts the words "Infinity" and "NaN"; those are not numbers a person typed
            return !string.IsNullOrWhiteSpace(text)
                && double.TryParse(Normalize(text), NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                && !double.IsInfinity(value) && !double.IsNaN(value);
        }
    }
}
