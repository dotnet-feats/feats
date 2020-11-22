using System;
using System.Threading.Tasks;
using Feats.Domain;
using Feats.Domain.Strategies;

namespace Feats.Evaluations.Strategies
{
    public interface IEvaluateStrategy<TStrategy>
        where TStrategy : IStrategy
    {
        // No additional params, how am I going to ever be able to assess on complex data, 
        // why author whyyyy did you did to meee? 
        // If you want a complex strategy, use the DI to get a hold of HttpContextAccessor, 
        // like any grownups would do and extract your additional requirements / data from there.
        //
        // This domain is meant to be used by 2 web applications: meaning, if for some 
        // weird reason, you want to extract this and put it in a stand alone console app, 
        // I'm worried about your sanity...
        Task<bool> IsOn<TFeature>(TFeature feature)
            where TFeature : IFeature;
    }
}