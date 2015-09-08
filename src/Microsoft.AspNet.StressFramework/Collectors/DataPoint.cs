using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public class DataPoint
    {
        private DataPoint(DateTime timestampUtc, object value)
        {
            TimestampUtc = timestampUtc;
            Value = value;
        }

        public DateTime TimestampUtc { get; }
        public object Value { get; }

        public static DataPoint Create(object value)
        {
            return new DataPoint(DateTime.UtcNow, value);
        }

        public override string ToString()
        {
            return $"[{TimestampUtc.ToLocalTime().ToString("O")}] {Value}";
        }
    }
}
