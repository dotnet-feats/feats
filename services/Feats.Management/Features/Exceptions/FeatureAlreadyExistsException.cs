using System;

namespace Feats.Management.Features.Exceptions
{
    public class FeatureAlreadyExistException : System.Exception
    {
        public FeatureAlreadyExistException() { }
        public FeatureAlreadyExistException(string message) : base(message) { }
        public FeatureAlreadyExistException(string message, System.Exception inner) : base(message, inner) { }
        protected FeatureAlreadyExistException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
