using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Feats.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public abstract class ValidationException : System.Exception
    {
        public ValidationException() { }
        
        public ValidationException(string message) : base(message) { }
        
        public ValidationException(string message, System.Exception inner) : base(message, inner) { }
        
        protected ValidationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }

    [ExcludeFromCodeCoverage]
    public sealed class ArgumentValidationException : ValidationException
    {
        public ArgumentValidationException(string message) 
        : base($"Missing argument {message}")
        { }
    }
}