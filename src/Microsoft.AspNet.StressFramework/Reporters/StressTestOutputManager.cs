using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public static class StressTestOutputManager
    {
        private const string FileFormat = "{displayName}.{uniqueID}.metrics.csv";
        private const string OutputDirectoryName = "TestOutput";

        public static IReadOnlyList<Metric> ReadBaseline(ITest test)
        {
            var fileLocation = GetFileLocation(test);

            // Assume the directory and file exists, if not we'll explode with a more informative error.
            var fileLines = File.ReadAllLines(fileLocation);

            try
            {
                var metrics = new List<Metric>();

                // File format is CSV, start at index 1 to avoid the columns
                for (var i = 1; i < fileLines.Length; i++)
                {
                    var data = fileLines[i].Split(',');
                    var iteration = int.Parse(data[0]);

                    var elapsedTimeTicks = long.Parse(data[4]);
                    var elapsedTime = new ElapsedTime(TimeSpan.FromTicks(elapsedTimeTicks));
                    var elapsedTimeMetric = Metric.Create(iteration, elapsedTime);
                    metrics.Add(elapsedTimeMetric);

                    var heapMemoryBytes = long.Parse(data[1]);
                    var workingSet = long.Parse(data[2]);
                    var privateBytes = long.Parse(data[3]);
                    var memoryUsage = new MemoryUsage(heapMemoryBytes, workingSet, privateBytes);
                    var memoryUsageMetric = Metric.Create(iteration, memoryUsage);
                    metrics.Add(memoryUsageMetric);
                }

                return metrics;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(
                    $"Could not parse test file '{fileLocation}':{Environment.NewLine}Error: {ex.Message}", ex);
            }
        }

        public static string WriteTestOutput(IReadOnlyList<Metric> metrics, ITest test)
        {
            var outputDirectory = GetOutputDirectoryLocation();
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var fileLocation = GetFileLocation(test);
            using (var writer = new StreamWriter(new FileStream(fileLocation, FileMode.Create)))
            {
                writer.WriteLine("Iteration,Heap,WorkingSet,PrivateBytes,ElapsedTicks");

                foreach (var iteration in metrics.GroupBy(g => g.Iteration).OrderBy(g => g.Key))
                {
                    var elapsed = (ElapsedTime)iteration.Single(m => m.Value is ElapsedTime).Value;
                    var mem = (MemoryUsage)iteration.Single(m => m.Value is MemoryUsage).Value;
                    writer.WriteLine($"{iteration.Key},{mem.HeapMemoryBytes},{mem.WorkingSet},{mem.PrivateBytes},{elapsed.Elapsed.Ticks}");
                }
            }

            return fileLocation;
        }

        private static string GetFileLocation(ITest test)
        {
            var outputDirectory = GetOutputDirectoryLocation();
            var outputFile = Path.Combine(
                outputDirectory,
                string.Format(FileFormat, test.TestCase.DisplayName, test.TestCase.UniqueID));

            return outputFile;
        }

        private static string GetOutputDirectoryLocation()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), OutputDirectoryName);
        }
    }
}
