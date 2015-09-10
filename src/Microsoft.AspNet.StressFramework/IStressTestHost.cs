namespace Microsoft.AspNet.StressFramework
{
    public interface IStressTestHost
    {
        void Setup();

        void Run(StressTestHostContext context);
    }
}
