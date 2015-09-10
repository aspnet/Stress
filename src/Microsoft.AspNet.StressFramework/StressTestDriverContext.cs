using System.Reflection;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestDriverContext
    {
        public MethodInfo TestMethod { get; set; }

        public StressRunSetup Setup { get; set; }
    }
}