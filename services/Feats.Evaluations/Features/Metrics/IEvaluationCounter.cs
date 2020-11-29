using Prometheus;

namespace Feats.Evaluations.Features.Metrics
{
    public interface IEvaluationCounter
    {
        void Inc(string feature, bool status);
    }

    public sealed class EvaluationCounter : IEvaluationCounter
    {
        private readonly Counter _count;

        public EvaluationCounter()
        {
            this._count = Prometheus.Metrics
                .CreateCounter(
                    "feats_evaluations", 
                    "Number ofevaluations for individual features.",
                    new CounterConfiguration
                    {
                        // Here you specify only the names of the labels.
                        LabelNames = new[] { "feature", "status" }
                    });
        }

        public void Inc(string feature, bool status)
        {
            this._count
                .WithLabels(feature, status.ToString().ToLowerInvariant())
                .Inc();
        }
    }
}