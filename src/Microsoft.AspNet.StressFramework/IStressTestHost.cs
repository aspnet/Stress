using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public interface IStressTestHost
    {
        void Run(StressTestHostContext context);
    }
}
