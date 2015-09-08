// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Microsoft.AspNet.StressFramework
{
    public class StressTestDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public StressTestDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public virtual IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo attribute)
        {
            var tests = new List<IXunitTestCase>();
            tests.Add(new StressTestCase(
                attribute.GetNamedArgument<int>(nameof(StressTestAttribute.Iterations)),
                attribute.GetNamedArgument<int>(nameof(StressTestAttribute.WarmupIterations)),
                _messageSink,
                testMethod,
                null));

            return tests;
        }
    }
}