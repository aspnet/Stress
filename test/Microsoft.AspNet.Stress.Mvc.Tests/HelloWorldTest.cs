// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Stress.Mvc.Tests;
using Microsoft.AspNet.StressFramework;
using Xunit;

namespace Microsoft.AspNet.Stress.Tests
{
    public class HelloWorldTest : IClassFixture<HelloWorldFixture>
    {
        public HelloWorldTest(HelloWorldFixture fixture)
        {
            Fixture = fixture;
        }

        public MvcFixture Fixture { get; }

        [StressTest]
        public async Task Index()
        {
            // Act
            var result = await Fixture.Client.GetAsync("");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
