using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNet.StressFramework.Reporters
{
    public class StressTestRunnerCSVReporter : DefaultRunnerReporter
    {
        public override string RunnerSwitch => "stresscsv";

        public override string Description => "output stress test results to CSV files. 1 CSV file per test.";

        public override IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new StressTestRunnerCSVMessageHandler(new ConsoleRunnerLogger(useColors: true));
        }
    }
}
