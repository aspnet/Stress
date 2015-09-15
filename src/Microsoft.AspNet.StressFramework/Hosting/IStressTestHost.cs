using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public interface IStressTestHost
    {
        Process LaunchHost(MethodInfo method);

        Task StartAsync();

        Task WaitForExitAsync();

        Task ShutdownAsync();
    }
}
