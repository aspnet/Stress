using System.Reflection;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public class StressTestDriverContext
    {
        public IStressTestHost Host { get; set; }
        public MethodInfo TestMethod { get; set; }
        public StressRunSetup Setup { get; set; }
    }
}