// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.StressFramework;

namespace Microsoft.AspNet.Stress.Tests
{
    public class SmokeTest
    {
        [StressTest]
        public IStressTestHost SmokeyMcSmokeTest()
        {
            return new HostingModelTest();
        }
    }
}
