// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using HelloWorldMvc;
using Microsoft.AspNet.TestHost;

namespace Microsoft.AspNet.Stress.Mvc.Tests
{
    public class HelloWorldFixture : IDisposable
    {
        public HelloWorldFixture()
        {
            var startup = new Startup();
            Server = HostingStartup.CreateServer(
                startup.GetType(),
                startup.Configure,
                startup.ConfigureServices);
            Client = Server.CreateDefaultClient();
        }

        public TestServer Server { get; }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}