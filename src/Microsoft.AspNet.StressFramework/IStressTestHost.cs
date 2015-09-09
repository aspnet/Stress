namespace Microsoft.AspNet.StressFramework
{
    public interface IStressTestHost
    {
        void Setup(StressTestHostContext context);

        void Run(StressTestHostContext context);
    }
}
