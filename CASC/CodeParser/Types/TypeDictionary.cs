using System;
using System.Collections.Generic;

namespace CASC.CodeParser.Types
{
    internal sealed class TypeDictionary
    {
        private static TypeDictionary instance = null;
        private static readonly object padlock = new object();
        private static Dictionary<TypeKind, HashSet<Type>> _typeCollection = new Dictionary<TypeKind, HashSet<Type>>()
        {
            [TypeKind.Number] = new HashSet<Type> {
                typeof(int),
                typeof(decimal),
            }
        };

        private TypeDictionary() { }

        public static TypeDictionary Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new TypeDictionary();
                    return instance;
                }
            }
        }
        public HashSet<Type> this[TypeKind kind] => _typeCollection[kind];
    }
}