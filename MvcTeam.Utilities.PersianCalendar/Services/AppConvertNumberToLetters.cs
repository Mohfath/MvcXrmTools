using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.PersianCalendar.Services
{
    public static class AppConvertNumberToLetters
    {

        public static string ToLetters(object input)
        {
            var type = input.GetType();

            decimal number = 0;

            if (type.Name == "Int32")
                number = Convert.ToDecimal(input);

            if (type.Name == "Double")
                number = Convert.ToDecimal(input);

            if (type.Name == "Decimal")
                number = (decimal)(input);


            if (type.Name == "Money")
                number = ((Money)input).Value;

            if (type.Name == "Int64")
                number = Convert.ToDecimal(input);

            var output = PNumberTString.GetStr(number.ToString());

            return output;
        }

    }
    public static class PNumberTString
    {

        private static string[] yakan = new string[10] { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };

        private static string[] dahgan = new string[10] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };

        private static string[] dahyek = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };

        private static string[] sadgan = new string[10] { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };

        private static string[] basex = new string[5] { "", "هزار", "میلیون", "میلیارد", "تریلیون" };

        private static string getnum3(int num3)
        {

            string s = "";

            int d3, d12;

            d12 = num3 % 100;

            d3 = num3 / 100;

            if (d3 != 0)

                s = sadgan[d3] + " و ";

            if ((d12 >= 10) && (d12 <= 19))
            {

                s = s + dahyek[d12 - 10];

            }

            else
            {

                int d2 = d12 / 10;

                if (d2 != 0)

                    s = s + dahgan[d2] + " و ";

                int d1 = d12 % 10;

                if (d1 != 0)

                    s = s + yakan[d1] + " و ";

                s = s.Substring(0, s.Length - 3);

            };

            return s;

        }

        static string int2str(string snum)
        {

            string stotal = "";

            if (snum == "") return "";

            if (snum == "0")
            {

                return yakan[0];

            }

            else
            {

                snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');

                int L = snum.Length / 3 - 1;

                for (int i = 0; i <= L; i++)
                {

                    int b = int.Parse(snum.Substring(i * 3, 3));

                    if (b != 0)

                        stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";

                }

                stotal = stotal.Substring(0, stotal.Length - 3);

            }

            return stotal;

        }

        public static string GetStr(string number)
        {
            if (!number.Contains("."))

                return int2str(number);

            else
            {

                string[] str = number.Split('.');
                string result = int2str(str[0]);
                string right = str[1].TrimEnd(new char[] { '0' });
                if (right.Length > 0)
                {
                    result += " ممیز ";
                }

                result += int2str(right);

                switch (right.Length)
                {

                    case 1:

                        result += " دهم ";

                        break;

                    case 2:

                        result += " صدم ";

                        break;

                    case 3:

                        result += " هزارم ";

                        break;

                    case 4:

                        result += " ده هزارم ";

                        break;
                    case 5:

                        result += " صد هزارم ";

                        break;

                    default:

                        break;

                }
                return result;
            }

        }

    }

    public static class DateToPersian
    {
        public static string GetPersianDate(DateTime? date)
        {
            if(!date.HasValue)
            return "";

            var pc = new System.Globalization.PersianCalendar();
            return $"{pc.GetYear(date.Value)}/{pc.GetMonth(date.Value)}/{pc.GetDayOfMonth(date.Value)}";
        }
    }
}
