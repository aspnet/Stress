// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Collections.Generic;
using Microsoft.AspNet.StressFramework.Collectors;

#if DNXCORE50 || DNX451
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Infrastructure;
using XunitDiagnosticMessage = Xunit.DiagnosticMessage;
#else
using XunitDiagnosticMessage = Xunit.Sdk.DiagnosticMessage;
#endif

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestRunner : TestRunner<StressTestCase>
    {
        private readonly IList<ICollector> _collectors;

        public StressTestRunner(
            StressTestCase test,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource,
            IMessageSink diagnosticMessageSink,
            IList<ICollector> collectors)
            : base(
                  new StressTestTest() { DisplayName = displayName, TestCase = test },
                  messageBus,
                  ((ReflectionTypeInfo)test.TestMethod.TestClass.Class).Type,
                  constructorArguments,
                  ((ReflectionMethodInfo)test.TestMethod.Method).MethodInfo,
                  testMethodArguments,
                  skipReason,
                  aggregator,
                  cancellationTokenSource)
        {
            _collectors = collectors;
        }

        protected async override Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;

            TestOutputHelper testOutputHelper = null;
            foreach (object obj in ConstructorArguments)
            {
                testOutputHelper = obj as TestOutputHelper;
                if (testOutputHelper != null)
                {
                    break;
                }
            }

            if (testOutputHelper != null)
            {
                testOutputHelper.Initialize(MessageBus, Test);
            }

            var instance = Activator.CreateInstance(TestClass, ConstructorArguments);

            foreach (var collector in _collectors)
            {
                collector.Initialize();
            }

            try
            {
                for (int i = 0; i < TestCase.WarmupIterations; i++)
                {
                    await InvokeTestMethodAsync(instance);
                }

                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < TestCase.Iterations; i++)
                {
                    var context = new StressTestIterationContext(i, _collectors, MessageBus);
                    context.BeginIteration();

                    await InvokeTestMethodAsync(instance);

                    context.EndIteration();
                }
                stopwatch.Stop();

                var executionTime = (decimal)stopwatch.Elapsed.TotalSeconds;

                if (testOutputHelper != null)
                {
                    output = testOutputHelper.Output;
                    testOutputHelper.Uninitialize();
                }

                return Tuple.Create(executionTime, output);
            }
            catch (Exception ex)
            {
                Aggregator.Add(ex);
                return Tuple.Create(0m, output);
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
            }
        }

        private async Task InvokeTestMethodAsync(object instance)
        {
            var result = TestMethod.Invoke(instance, TestMethodArguments);
            var task = result as Task;
            if (task != null)
            {
                await task;
            }
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

        private class StressTestTest : ITest
        {
            public string DisplayName { get; set; }

            public ITestCase TestCase { get; set; }
        }
    }
}