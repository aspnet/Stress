using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework.Hosting
{
    public interface IStressTestDriver
    {
        Task RunAsync(StressRunSetup setup);
    }
}