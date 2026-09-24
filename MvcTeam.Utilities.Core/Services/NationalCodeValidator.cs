using System.Linq;
using System.Text;

namespace MvcTeam.Utilities.Services
{
    public static class NationalCodeValidator
    {
        //Iranian national code (کد ملی): 10 digits, the last one is a check digit.
        //Spaces and dashes are ignored; Persian and Arabic digits count as 0-9.
        public static bool IsValid(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode)) return false;

            var digits = new StringBuilder(10);
            foreach (var c in nationalCode)
            {
                if (char.IsWhiteSpace(c) || c == '-') continue;
                var digit = ToDigit(c);
                if (digit < 0) return false;
                digits.Append((char)('0' + digit));
            }

            if (digits.Length != 10) return false;
            var code = digits.ToString();

            //Codes like 1111111111 pass the checksum but are not issued
            if (code.All(c => c == code[0])) return false;

            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                sum += (code[i] - '0') * (10 - i);
            }
            var remainder = sum % 11;
            var checkDigit = code[9] - '0';

            return remainder < 2 ? checkDigit == remainder : checkDigit == 11 - remainder;
        }

        private static int ToDigit(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= '۰' && c <= '۹') return c - '۰'; //Persian ۰-۹
            if (c >= '٠' && c <= '٩') return c - '٠'; //Arabic ٠-٩
            return -1;
        }
    }
}
