using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public interface IEvaluateStrategy<TStrategy>
        where TStrategy : IStrategy
    {
        // decided not to give an http context to all my strats, still questioning my sanity
        Task<bool> IsOn(TStrategy strategy, IDictionary<string, string> values = null);
    }
}