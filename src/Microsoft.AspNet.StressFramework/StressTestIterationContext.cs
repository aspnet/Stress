using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNet.StressFramework.Collectors;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestIterationContext
    {
        private IList<ICollector> _collectors;
        private List<DataPoint> _recordings = new List<DataPoint>();
        private Stopwatch _stopwatch;

        public StressTestIterationContext(IList<ICollector> collectors)
        {
            _collectors = collectors;
        }

        /// <summary>
        /// Record a data point
        /// </summary>
        public void Record<T>(T data)
        {
            var dataPoint = DataPoint.Create(data);
            _recordings.Add(dataPoint);
        }

        public void BeginIteration()
        {
            foreach (var collector in _collectors)
            {
                collector.BeginIteration(this);
            }

            _stopwatch = Stopwatch.StartNew();
        }

        public void EndIteration()
        {
            _stopwatch.Stop();

            foreach (var collector in _collectors)
            {
                collector.EndIteration(this);
            }
        }
    }
}