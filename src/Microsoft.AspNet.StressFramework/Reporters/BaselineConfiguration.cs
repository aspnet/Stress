using Microsoft.Framework.Configuration;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class BaselineConfiguration
    {
        private const string BaselineConfigurationSectionName = "baselineConfig";
        private const string DefaultFolder = "StressBaselines";
        private const int DefaultHeapMemoryFailPercentThreshold = 100;

        public BaselineConfiguration()
        {
            Folder = DefaultFolder;
            HeapMemoryFailPercentThreshold = DefaultHeapMemoryFailPercentThreshold;
        }

        public BaselineConfiguration(IConfigurationRoot configuration)
            : this()
        {
            var baselineConfig = configuration.GetSection(BaselineConfigurationSectionName);
            if (baselineConfig != null)
            {
                var folder = baselineConfig[nameof(Folder)];
                if (string.IsNullOrEmpty(folder))
                {
                    Folder = folder;
                }

                var failPercentThresholdString = baselineConfig[nameof(HeapMemoryFailPercentThreshold)];
                if (string.IsNullOrEmpty(failPercentThresholdString))
                {
                    HeapMemoryFailPercentThreshold = int.Parse(failPercentThresholdString);
                }
            }
        }

        public string Folder { get; }

        public int HeapMemoryFailPercentThreshold { get; }
    }
}
