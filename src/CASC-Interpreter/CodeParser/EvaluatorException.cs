using System;
using System.Runtime.Serialization;

namespace CASC.CodeParser
{
    [Serializable]
    public class EvaluatorException : Exception
    {
        public EvaluatorException() { }
        public EvaluatorException(string message) : base(message) { }
        public EvaluatorException(string message, Exception inner) : base(message, inner) { }
        protected EvaluatorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}