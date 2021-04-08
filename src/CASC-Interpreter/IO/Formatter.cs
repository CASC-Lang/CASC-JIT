using System;
using System.Collections.Generic;
using System.Linq;

namespace CASC.IO
{
    public sealed class Formatter
    {
        public static string Format(object obj) => obj switch
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
                return string.Format("[{0}]", string.Join(", ", array.Select(obj => Format(obj))));
            }))(),
            _ => throw new Exception($"Formatter does not support type '{obj.GetType()}'")
        };
    }
}