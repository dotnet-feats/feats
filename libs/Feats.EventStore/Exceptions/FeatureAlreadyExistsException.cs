using System;
using Feats.Domain;
using Feats.Domain.Exceptions;

namespace Feats.EventStore.Exceptions
{
    public class FeatureAlreadyExistsException : ConflictException
    {
    }
}
