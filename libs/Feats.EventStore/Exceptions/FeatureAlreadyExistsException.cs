using System;
using Feats.Domain;

namespace Feats.EventStore.Exceptions
{
    public class FeatureAlreadyExistsException : ConflictException
    {
        public FeatureAlreadyExistsException()
            : base()
        {
        }
    }
}
