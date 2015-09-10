using System;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class StressTestContext
    {
        public int WarmupIterations { get; set; }

        public int Iterations { get; set; }
    }
}