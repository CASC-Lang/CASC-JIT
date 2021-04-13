using System;
using System.Collections.Generic;
using System.Linq;
using CASC.CodeParser.Symbols;

namespace CASC.IO
{
    public sealed class Formatter
    {
        public static string FormatValue(object obj)
        {
            if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = obj.GetType().GetGenericArguments()[0];

                if (genericType == typeof(decimal))
                    return string.Format("[{0}]", string.Join(", ", ((List<decimal>)obj).Select(o => FormatValue(o))));
                if (genericType == typeof(bool))
                    return string.Format("[{0}]", string.Join(", ", ((List<bool>)obj).Select(o => FormatValue(o))));
                if (genericType == typeof(string))
                    return string.Format("[{0}]", string.Join(", ", ((List<string>)obj).Select(o => FormatValue(o))));
                if (genericType == typeof(object))
                    return string.Format("[{0}]", string.Join(", ", ((List<object>)obj).Select(o => FormatValue(o))));
                if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    dynamic o = obj;
                    var builder = "[";
                    foreach (var o1 in o)
                    {
                        builder += $"{FormatValue(o1)}, ";
                    }
                    builder = builder.Substring(0, builder.Length - 2);
                    return builder + "]";
                }
            }

            return obj switch
            {
                decimal number => number.ToString(),
                bool boolean => boolean ? "true" : "false",
                string str => string.IsNullOrEmpty(str) ? "" : $"\"{str}\"",
                _ => throw new Exception($"Formatter does not support type '{obj.GetType()}'")
            };
        }

        // HARD CODED! This should replace with more dynamic way.
        public static string FormatType(object obj)
        {
            if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = obj.GetType().GetGenericArguments()[0];

                if (genericType == typeof(decimal)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Number.Name}>";
                if (genericType == typeof(bool)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Bool.Name}>";
                if (genericType == typeof(string)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.String.Name}>";
                if (genericType == typeof(object)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Any.Name}>";
                if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(List<>)) return $"{TypeSymbol.Array.Name}<{FormatType(genericType)}>";
            }

            return obj switch
            {
                decimal d => TypeSymbol.Number.Name,
                bool b => TypeSymbol.Bool.Name,
                string s => TypeSymbol.String.Name,
                _ => TypeSymbol.Error.Name
            };
        }

        // HARD CODED! This should replace with more dynamic way.
        public static string FormatType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericType = type.GetGenericArguments()[0];

                if (genericType == typeof(decimal)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Number.Name}>";
                if (genericType == typeof(bool)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Bool.Name}>";
                if (genericType == typeof(string)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.String.Name}>";
                if (genericType == typeof(object)) return $"{TypeSymbol.Array.Name}<{TypeSymbol.Any.Name}>";
                if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(List<>)) return $"{TypeSymbol.Array.Name}<{FormatType(genericType)}>";
            }

            if (type == typeof(decimal)) return TypeSymbol.Number.Name;
            if (type == typeof(bool)) return TypeSymbol.Bool.Name;
            if (type == typeof(string)) return TypeSymbol.String.Name;
            else return TypeSymbol.Error.Name;
        }
    }
}