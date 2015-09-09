using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework;

namespace Microsoft.AspNet.Stress.Tests
{
    public class HostingModelTest : IStressTestHost
    {
        public void Run(StressTestHostContext context)
        {
            Console.WriteLine("Ran in the host!");
        }
    }
}
