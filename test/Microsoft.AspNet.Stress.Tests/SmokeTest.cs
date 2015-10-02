// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Xunit.Performance;

namespace Microsoft.AspNet.Stress.Tests
{
    public class SmokeTest
    {
        [Benchmark]
        [MeasureInstructionsRetired]
        public void SmokeyMcSmokeTest()
        {
            // Arrange
            var formData = "first=second&third=fourth&fifth=sixth";
            var formReader = new FormReader(formData);
            var pairs = new List<KeyValuePair<string, string>>();
            var cancellationToken = new CancellationToken();

            // Act
            Benchmark.IterateAsync(async () =>
            {
                var pair = await formReader.ReadNextPairAsync(cancellationToken);
                while (pair != null)
                {
                    pairs.Add(pair.Value);
                    pair = await formReader.ReadNextPairAsync(cancellationToken);
                }
            });
        }
    }
}
