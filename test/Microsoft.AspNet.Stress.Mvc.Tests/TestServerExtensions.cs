// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using Microsoft.AspNet.TestHost;

namespace Microsoft.AspNet.Stress.Mvc.Tests
{
    public static class TestServerExtensions
    {
        public static HttpClient CreateDefaultClient(this TestServer testServer)
        {
            var client = testServer.CreateClient();
            client.BaseAddress = new Uri("http://localhost");

            return client;
        }
    }
}
