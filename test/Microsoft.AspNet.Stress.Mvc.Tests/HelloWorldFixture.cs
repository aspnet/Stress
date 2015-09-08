// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Stress.Mvc.Tests
{
    public class HelloWorldFixture : MvcFixture
    {
        public HelloWorldFixture()
            : base(new HelloWorldMvc.Startup())
        {
        }
    }
}