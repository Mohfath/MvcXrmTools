using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace MvcTeam.Utilities.Workflows
{
    //Text operations: capitalize, pad, replace, substring, regex, upper/lower case, trim, strip spaces.
    //Based on StringFunctions from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko).
    public class StringFunctions : CodeActivity
    {
        [RequiredArgument]
        [Input("Input Text")]
        [Default("")]
        public InArgument<string> InputText { get; set; }

        [RequiredArgument]
        [Input("Capitalize All Words")]
        [Default("True")]
        public InArgument<bool> CapitalizeAllWords { get; set; }

        [RequiredArgument]
        [Input("Padding: Pad Character")]
        [Default("")]
        public InArgument<string> PadCharacter { get; set; }

        [RequiredArgument]
        [Input("Padding: Pad on the Left")]
        [Default("False")]
        public InArgument<bool> PadOnTheLeft { get; set; }

        [RequiredArgument]
        [Input("Padding: Final Length")]
        [Default("10")]
        public InArgument<int> FinalLengthWithPadding { get; set; }

        [RequiredArgument]
        [Input("Replace: Old Value")]
        [Default("")]
        public InArgument<string> ReplaceOldValue { get; set; }

        [Input("Replace: New Value")]
        [Default("")]
        public InArgument<string> ReplaceNewValue { get; set; }

        [RequiredArgument]
        [Input("Replace: Case Sensitive")]
        [Default("False")]
        public InArgument<bool> CaseSensitive { get; set; }

        [RequiredArgument]
        [Input("Substring: From Left to Right")]
        [Default("True")]
        public InArgument<bool> FromLeftToRight { get; set; }

        [RequiredArgument]
        [Input("Substring: Start Index")]
        [Default("0")]
        public InArgument<int> StartIndex { get; set; }

        [RequiredArgument]
        [Input("Substring: Length")]
        [Default("3")]
        public InArgument<int> SubStringLength { get; set; }

        [RequiredArgument]
        [Input("Regular Expression")]
        [Default("")]
        public InArgument<string> RegularExpression { get; set; }

        [Output("Capitalized Text")]
        public OutArgument<string> CapitalizedText { get; set; }

        [Output("Text Length")]
        public OutArgument<int> TextLength { get; set; }

        [Output("Padded Text")]
        public OutArgument<string> PaddedText { get; set; }

        [Output("Replaced Text")]
        public OutArgument<string> ReplacedText { get; set; }

        [Output("Substring Text")]
        public OutArgument<string> SubstringText { get; set; }

        [Output("Trimmed Text")]
        public OutArgument<string> TrimmedText { get; set; }

        [Output("Regex Success")]
        public OutArgument<bool> RegexSuccess { get; set; }

        [Output("Regex Text")]
        public OutArgument<string> RegexText { get; set; }

        [Output("Uppercase Text")]
        public OutArgument<string> UppercaseText { get; set; }

        [Output("Lowercase Text")]
        public OutArgument<string> LowercaseText { get; set; }

        [Output("Without Spaces")]
        public OutArgument<string> WithoutSpaces { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
            var inputText = InputText.Get(context);
            if (inputText == null) inputText = "";

            var capitalizedText = "";
            var paddedText = "";
            var replacedText = "";
            var subStringText = "";
            var regexText = "";
            var uppercaseText = "";
            var lowercaseText = "";
            var withoutSpaces = "";
            var regexSuccess = false;

            Services.StringFunctions.Apply(
                CapitalizeAllWords.Get(context), inputText, PadCharacter.Get(context), PadOnTheLeft.Get(context),
                FinalLengthWithPadding.Get(context), CaseSensitive.Get(context), ReplaceOldValue.Get(context),
                ReplaceNewValue.Get(context), SubStringLength.Get(context), StartIndex.Get(context),
                FromLeftToRight.Get(context), RegularExpression.Get(context),
                out capitalizedText, out paddedText, out replacedText, out subStringText, out regexText,
                out regexSuccess, out uppercaseText, out lowercaseText, out withoutSpaces);

            context.GetExtension<ITracingService>().Trace("StringFunctions on '{0}': capitalized={1}", inputText, capitalizedText);

            CapitalizedText.Set(context, capitalizedText);
            TextLength.Set(context, capitalizedText.Length);
            PaddedText.Set(context, paddedText);
            ReplacedText.Set(context, replacedText);
            SubstringText.Set(context, subStringText);
            TrimmedText.Set(context, inputText.Trim());
            RegexSuccess.Set(context, regexSuccess);
            RegexText.Set(context, regexText);
            UppercaseText.Set(context, uppercaseText);
            LowercaseText.Set(context, lowercaseText);
            WithoutSpaces.Set(context, withoutSpaces);
        }
    }
}