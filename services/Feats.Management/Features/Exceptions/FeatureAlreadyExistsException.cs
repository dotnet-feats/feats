using System;
using Feats.Domain.Exceptions;

namespace Feats.Management.Features.Exceptions
{
    public sealed class FeatureAlreadyExistsException : ConflictException
    {
    }
}
