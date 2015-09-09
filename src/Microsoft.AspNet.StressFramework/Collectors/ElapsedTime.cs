using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public struct ElapsedTime
    {
        public TimeSpan Elapsed { get; }
        
        public ElapsedTime(TimeSpan elapsed)
        {
            Elapsed = elapsed;
        }
    }
}
