using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Feats.Domain
{
    [ExcludeFromCodeCoverage]
    public abstract class NotFoundException : System.Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected NotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    }
}