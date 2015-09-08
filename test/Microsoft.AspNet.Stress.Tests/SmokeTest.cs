// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.StressFramework;
using Microsoft.AspNet.StressFramework.Collectors;
using Microsoft.AspNet.WebUtilities;
using Xunit;

namespace Microsoft.AspNet.Stress.Tests
{
    public class SmokeTest
    {
        [StressTest]
        [TotalMemoryCollector]
        public void SmokeyMcSmokeTest()
        {
            Assert.True(true);
        }

        [StressTest]
        public async Task SmokeyHttpAbstractionsSmokeTest()
        {
            // Arrange
            var formData = "first=second&third=fourth&fifth=sixth";
            var formReader = new FormReader(formData);
            var pairs = new List<KeyValuePair<string, string>>();
            var cancellationToken = new CancellationToken();
            var expectedPairs = new Dictionary<string, string>
            {
                { "first", "second" },
                { "third", "fourth" },
                { "fifth", "sixth" },
            }.ToList();

            // Act
            var pair = await formReader.ReadNextPairAsync(cancellationToken);
            while (pair != null)
            {
                pairs.Add(pair.Value);
                pair = await formReader.ReadNextPairAsync(cancellationToken);
            }

            // Assert
            Assert.Equal(expectedPairs, pairs);
        }
    }
}
