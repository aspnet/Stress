using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestIterationContext
    {
        private readonly IList<ICollector> _collectors;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly List<Metric> _recordings = new List<Metric>();
        private readonly IMessageBus _bus;
        private readonly Process _me;

        public IReadOnlyList<Metric> Recordings { get; }
        public int Iteration { get; }

        public StressTestIterationContext(int iteration, IList<ICollector> collectors, IMessageBus bus)
        {
            Iteration = iteration;

            _collectors = collectors;
            _bus = bus;
            _me = Process.GetCurrentProcess();

            // The read-only wrapper IS live updated by changes to the underlying list
            // https://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k(System.Collections.Generic.List`1.AsReadOnly);k(SolutionItemsProject);k(DevLang-csharp)&rd=true
            Recordings = _recordings.AsReadOnly();
        }

        /// <summary>
        /// Record a metric
        /// </summary>
        public void Record<T>(T data)
        {
            var metric = Metric.Create(Iteration, data);
            _recordings.Add(metric);
        }

        public void BeginIteration()
        {
            foreach (var collector in _collectors)
            {
                collector.BeginIteration(this);
            }

            // Start capturing elapsed time
            _stopwatch.Start();
        }

        public void EndIteration()
        {
            // Stop capturing elapsed time
            _stopwatch.Stop();

            // Record the elapsed time and memory usage
            _recordings.Add(Metric.Create(Iteration, new ElapsedTime(_stopwatch.Elapsed)));
        }
    }
}