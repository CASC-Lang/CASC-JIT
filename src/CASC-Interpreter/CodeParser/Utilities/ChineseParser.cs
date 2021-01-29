using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace CASC.CodeParser.Utilities
{
    public static class ChineseParser
    {
        private static readonly Dictionary<char, int> _zh2digitTable = new Dictionary<char, int>
        {
            {'零', 0}, {'一', 1}, {'二', 2}, {'三', 3}, {'四', 4}, {'五', 5}, {'六', 6}, {'七', 7}, {'八', 8}, {'九', 9}, {'十', 10},
            {'壹', 1}, {'貳', 2}, {'參', 3}, {'肆', 4}, {'伍', 5}, {'陆', 6}, {'柒', 7}, {'捌', 8}, {'玖', 9}, {'拾', 10},
            {'百', 100}, {'千', 1000}, {'萬', 10000}, {'億', 100000000}, {'佰', 100}, {'仟', 1000}
        };

        public static bool isDigit(char character)
        {
            if (char.IsDigit(character) || character == '.' || character == '點')
                return true;

            return _zh2digitTable.ContainsKey(character);
        }

        public static bool tryParseDigits(string str, out decimal value)
        {
            if (decimal.TryParse(str, out value))
                return true;

            var result = ParseDigitsFromChinese(str);
            value = result.value;

            return result.pass;
        }

        public static (bool pass, decimal value) ParseDigitsFromChinese(string str)
        {
            var point_counter = str.Length - Regex.Replace(str, "([.]|[點])", "").Length;

            decimal result;

            if (point_counter == 0)
                result = PraseDigitFromChinese(str);
            else if (point_counter == 1)
            {
                var nums = new Regex("([.]|[點])").Split(str);

                var left = PraseDigitFromChinese(nums[0]);
                var right = PraseDigitFromChinese(nums[2]);

                decimal.TryParse($"{left}.{right}", out result);
            }
            else
                throw new Exception($"ERROR: String \"{str}\" is not valid chinese numeral.");


            return (true, result);
        }

        private static decimal PraseDigitFromChinese(string str)
        {
            var digitNum = 0;
            var result = 0;
            var tmp = 0;
            var billion = 0;

            while (digitNum < str.Length)
            {
                var tmpZH = str[digitNum];
                if (!_zh2digitTable.ContainsKey(tmpZH))
                    throw new Exception($"ERROR: String \"{str}\" is not valid chinese numeral.");

                var tmpNum = _zh2digitTable[tmpZH];

                if (tmpNum == 100000000)
                {
                    result += tmp;
                    result *= tmpNum;
                    billion *= 100000000;
                    billion += result;
                    result = 0;
                    tmp = 0;
                }
                else if (tmpNum == 10000)
                {
                    result += tmp;
                    result *= tmpNum;
                    tmp = 0;
                }
                else if (tmpNum >= 10)
                {
                    if (tmp == 0)
                        tmp = 1;

                    result += tmpNum * tmp;
                    tmp = 0;
                }
                else
                {
                    tmp *= 10;
                    tmp += tmpNum;
                }


                digitNum += 1;
            }

            result += tmp;
            result += billion;

            return result;
        }
    }
}