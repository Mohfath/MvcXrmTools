using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Services
{
    //Every text operation offered by the StringFunctions workflow step, as a stateless helper.
    //Based on StringFunctions from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko).
    public static class StringFunctions
    {
        public static void Apply(bool capitalizeAllWords, string inputText, string padCharacter, bool padontheLeft,
            int finalLengthwithPadding, bool caseSensitive, string replaceOldValue, string replaceNewValue,
            int subStringLength, int startIndex, bool fromLefttoRight, string regularExpression,
            out string capitalizedText, out string paddedText, out string replacedText, out string subStringText,
            out string regexText, out bool regexSuccess, out string uppercaseText, out string lowercaseText,
            out string withoutSpaces)
        {
            if (inputText == null) inputText = "";
            if (replaceNewValue == null) replaceNewValue = "";
            if (padCharacter == null) padCharacter = "";
            if (regularExpression == null) regularExpression = "";
            if (replaceOldValue == null) replaceOldValue = "";

            //Capitalize
            if (capitalizeAllWords)
                capitalizedText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inputText);
            else
                capitalizedText = inputText.Length == 0 ? "" : char.ToUpper(inputText[0]) + inputText.Substring(1);

            //Padding
            var pad = padCharacter.Length == 0 ? ' ' : padCharacter[0];
            paddedText = padontheLeft
                ? inputText.PadLeft(finalLengthwithPadding, pad)
                : inputText.PadRight(finalLengthwithPadding, pad);

            //Replace
            if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(replaceOldValue))
                replacedText = inputText;
            else if (caseSensitive)
                replacedText = inputText.Replace(replaceOldValue, replaceNewValue);
            else
                replacedText = CompareAndReplace(inputText, replaceOldValue, replaceNewValue, StringComparison.OrdinalIgnoreCase);

            //Substring
            if (subStringLength <= 0 || startIndex < 0 || startIndex >= inputText.Length)
                subStringText = "";
            else
            {
                if (!fromLefttoRight)
                    startIndex = inputText.Length - subStringLength - startIndex;
                if (startIndex < 0) startIndex = 0;
                if (startIndex >= inputText.Length)
                    subStringText = "";
                else
                {
                    if (startIndex + subStringLength > inputText.Length)
                        subStringLength = inputText.Length - startIndex;
                    subStringText = inputText.Substring(startIndex, subStringLength);
                }
            }

            //Regex
            regexText = "";
            regexSuccess = false;
            if (!string.IsNullOrEmpty(regularExpression))
            {
                var regex = new Regex(regularExpression);
                var match = regex.Match(inputText);
                if (match.Success)
                {
                    regexSuccess = true;
                    regexText = match.Value;
                }
            }

            uppercaseText = inputText.ToUpper();
            lowercaseText = inputText.ToLower();
            withoutSpaces = inputText.Replace(" ", "");
        }

        //Replace with the given comparison (used for the case-insensitive replace); a manual scan must use >= 0, not > 0
        //so that a match at the start of the string is found
        private static string CompareAndReplace(string text, string old, string replacement, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(old)) return text;

            var result = new StringBuilder();
            var oldLength = old.Length;
            var pos = 0;
            var next = text.IndexOf(old, comparison);

            while (next >= 0)
            {
                result.Append(text, pos, next - pos);
                result.Append(replacement);
                pos = next + oldLength;
                next = text.IndexOf(old, pos, comparison);
            }

            result.Append(text, pos, text.Length - pos);
            return result.ToString();
        }
    }
}