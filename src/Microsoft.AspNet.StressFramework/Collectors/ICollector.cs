namespace Microsoft.AspNet.StressFramework.Collectors
{
    public interface ICollector
    {
        /// <summary>
        /// Called by the stress framework to initialize the collector, before any iterations have been run
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called by the stress framework before a non-warmup iteration is executed.
        /// </summary>
        void BeginIteration(StressTestIterationContext context);

        /// <summary>
        /// Called by the stress framework after a non-warmup iteration is executed.
        /// </summary>
        void EndIteration(StressTestIterationContext context);
    }
}