// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.StressFramework;
using Xunit;

namespace Microsoft.AspNet.Stress.Tests
{
    public class MvcSmokeTest
    {
        [StressTest]
        public StressRunSetup IndexTest()
        {
            var applicationPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "websites", "HelloWorldMvc");
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5001/");

            return StressRunSetup.CreateClientServerTest(applicationPath,
                async () =>
                {

                    var result = await client.GetAsync("Home/Index");

                    // Assert
                    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                    Assert.Equal("Hello World!", await result.Content.ReadAsStringAsync());
                });
        }
    }
}
