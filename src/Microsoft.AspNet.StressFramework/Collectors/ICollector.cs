using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Collectors
{
    public interface ICollector
    {
        /// <summary>
        /// Called by the stress framework to initialize the collector
        /// </summary>
        void Initialize(Process target, CollectorContext context);

        /// <summary>
        /// Called by the stress framework to stop the collector
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
    }
}