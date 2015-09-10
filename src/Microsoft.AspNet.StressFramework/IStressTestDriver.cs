using System.Threading.Tasks;

namespace Microsoft.AspNet.StressFramework
{
    public interface IStressTestDriver
    {
        void Setup(StressTestDriverContext context);

        Task RunAsync(StressTestDriverContext context);
    }
}