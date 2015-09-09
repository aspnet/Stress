namespace Microsoft.AspNet.StressFramework
{
    public interface IStressTestDriver
    {
        void Setup();

        void Run(int driverIterations);
    }
}