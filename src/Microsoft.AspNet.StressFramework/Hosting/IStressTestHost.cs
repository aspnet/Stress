using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public interface IStressTestHost
    {
        Process LaunchHost(MethodInfo method);

        void Start();

        Task WaitForExitAsync();

        Task ShutdownAsync();
    }
}
