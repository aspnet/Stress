// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework.Collectors;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestCase : XunitTestCase
    {
#if DNX451
        [NonSerialized]
#endif
        private readonly List<ICollector> _collectors;

#if DNX451
        [NonSerialized]
#endif
        private readonly IMessageSink _diagnosticMessageSink;

        public StressTestCase(
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

            // Load collectors
            _collectors = TestMethod.Method
                .GetCustomAttributes(typeof(ICollector))
                .OfType<ReflectionAttributeInfo>()
                .Select(r => r.Attribute)
                .OfType<ICollector>()
                .ToList();

            _diagnosticMessageSink = diagnosticMessageSink;
            DisplayName = suppliedDisplayName ?? BaseDisplayName;
            Iterations = iterations;
            WarmupIterations = warmupIterations;

            TestMethodArguments = testMethodArguments?.ToArray();
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
                _diagnosticMessageSink,
                _collectors);
            return runner.RunAsync();
        }
    }

}