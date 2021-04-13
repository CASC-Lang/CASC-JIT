using System;
using System.Collections.Generic;
using System.Linq;
using CASC.CodeParser.Symbols;

namespace CASC.IO
{
    public sealed class Formatter
    {
        public static string FormatValue(object obj) => obj switch
        {
            decimal number => ((Func<string>)(() =>
            {
                return number.ToString();
            }))(),
            bool boolean => ((Func<string>)(() =>
            {
                return boolean ? "true" : "false";
            }))(),
            string str => ((Func<string>)(() =>
            {
                return string.IsNullOrEmpty(str) ? "" : $"\"{str}\"";
            }))(),
            List<object> array => ((Func<string>)(() =>
            {
                return string.Format("[{0}]", string.Join(", ", array.Select(obj => FormatValue(obj))));
            }))(),
            _ => throw new Exception($"Formatter does not support type '{obj.GetType()}'")
        };

        public static string FormatType(object obj) => obj switch {
            decimal d => TypeSymbol.Number.Name,
            bool b => TypeSymbol.Bool.Name,
            string s => TypeSymbol.String.Name,
            List<object> array => ((Func<string>)(() =>
            {
                return string.Format("{0}<{1}>", TypeSymbol.Array.Name, string.Join(", ", array.GetType().GetGenericArguments().Select(obj => FormatType(obj))));
            }))(),
            _ => TypeSymbol.Error.Name
        };
    }
}