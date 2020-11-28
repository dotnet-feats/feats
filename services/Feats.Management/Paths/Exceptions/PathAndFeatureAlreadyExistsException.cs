using System;
using Feats.Domain;

namespace Feats.Management.Features.Exceptions
{
    public class PathAndFeatureAlreadyExistsException : ConflictException
    {
        public PathAndFeatureAlreadyExistsException()
            : base()
        {
        }
    }
}
