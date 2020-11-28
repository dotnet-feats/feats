using System;
using Feats.Domain;

namespace Feats.Management.Features.Exceptions
{
    public class FeatureAlreadyExistsException : ConflictException
    {
        public FeatureAlreadyExistsException()
            : base()
        {
        }
    }
}
