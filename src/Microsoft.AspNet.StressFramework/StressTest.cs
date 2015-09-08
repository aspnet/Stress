// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTest : XunitTestCase
    {
#if DNX451
        [NonSerialized]
#endif
        private readonly IMessageSink _diagnosticMessageSink;

        public StressTest(
            int iterations,
            int warmupIterations,
            IMessageSink diagnosticMessageSink,
            ITestMethod testMethod,
            object[] testMethodArguments)
            : base(diagnosticMessageSink, Xunit.Sdk.TestMethodDisplay.Method, testMethod, null)
        {
            var suppliedDisplayName = TestMethod.Method.GetCustomAttributes(typeof(FactAttribute))
                .First()
                .GetNamedArgument<string>("DisplayName");

            _diagnosticMessageSink = diagnosticMessageSink;
            DisplayName = suppliedDisplayName ?? BaseDisplayName;
            Iterations = iterations;
            WarmupIterations = warmupIterations;
            
            TestMethodArguments = testMethodArguments.ToArray();
        }

        public int Iterations { get; private set; }

        public int WarmupIterations { get; private set; }

        public override Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            var runner = new StressTestRunner(
                this,
                DisplayName,
                SkipReason,
                constructorArguments,
                TestMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource,
                _diagnosticMessageSink);
            return runner.RunAsync();
        }
    }

}