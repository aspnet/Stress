// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Diagnostics;

#if DNXCORE50 || DNX451
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Infrastructure;
using XunitDiagnosticMessage = Xunit.DiagnosticMessage;
#else
using XunitDiagnosticMessage = Xunit.Sdk.DiagnosticMessage;
#endif

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestRunner : XunitTestCaseRunner
    {
        private static string _machineName = GetMachineName();
        private static string _framework = GetFramework();
        private readonly IMessageSink _diagnosticMessageSink;

        public StressTestRunner(
                StressTest test,
                string displayName,
                string skipReason,
                object[] constructorArguments,
                object[] testMethodArguments,
                IMessageBus messageBus,
                ExceptionAggregator aggregator,
                CancellationTokenSource cancellationTokenSource,
                IMessageSink diagnosticMessageSink)
            : base(
                  test,
                  displayName,
                  skipReason,
                  constructorArguments,
                  testMethodArguments,
                  messageBus,
                  aggregator,
                  cancellationTokenSource)
        {
            TestCase = test;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public new StressTest TestCase { get; private set; }

        protected override async Task<RunSummary> RunTestAsync()
        {
            
            for (int i = 0; i < TestCase.WarmupIterations; i++)
            {
                var runner = CreateRunner(i + 1, TestCase.WarmupIterations,  warmup: true);
                await runner.RunAsync();
            }

            var stopwatch = new Stopwatch();
            for (int i = 0; i < TestCase.Iterations; i++)
            {
                var runner = CreateRunner(i + 1, TestCase.Iterations, warmup: false);

                stopwatch.Start();

                // Running the actual test
                var result = await runner.RunAsync();

                stopwatch.Stop();
            }

            var runSummary = new RunSummary()
            {
                Total = 1,
                Time = (decimal)stopwatch.Elapsed.TotalSeconds,
            };
            return runSummary;
        }

        private XunitTestRunner CreateRunner(int iteration, int totalIterations, bool warmup)
        {
            var name = $"{DisplayName} [Stage: {(warmup ? "Warmup" : "Collection")}] [Iteration: {iteration}/{totalIterations}]";

            return new XunitTestRunner(
                new XunitTest(TestCase, name),
                MessageBus,
                TestClass,
                ConstructorArguments,
                TestMethod,
                TestMethodArguments,
                SkipReason,
                BeforeAfterAttributes,
                Aggregator,
                CancellationTokenSource);
        }

        private static string GetFramework()
        {
#if DNX451 || DNXCORE50
            var services = CallContextServiceLocator.Locator.ServiceProvider; 
            var env = (IRuntimeEnvironment)services.GetService(typeof(IRuntimeEnvironment)); 
            return "DNX." + env.RuntimeType;
#else
            return ".NETFramework";
#endif
        }

        private static string GetMachineName()
        {
            return Environment.GetEnvironmentVariable("COMPUTERNAME");
        }
    }
}