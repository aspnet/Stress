using System;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class Metric
    {
        private Metric(DateTime timestampUtc, int iteration, object value)
        {
            Value = value;
            Iteration = iteration;
            TimestampUtc = timestampUtc;
        }

        public object Value { get; }
        public int Iteration { get; }
        public DateTime TimestampUtc { get; }

        public static Metric Create(int iteration, object value)
        {
            return new Metric(DateTime.UtcNow, iteration, value);
        }

        public override string ToString()
        {
            return $"[{Iteration}|{TimestampUtc.ToLocalTime().ToString("O")}] {Value}";
        }
    }
}
