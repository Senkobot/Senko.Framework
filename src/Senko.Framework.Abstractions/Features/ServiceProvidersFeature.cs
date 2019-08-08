using System;
using Microsoft.AspNetCore.Http.Features;

namespace Senko.Framework.Features
{
    public struct  ServiceProvidersFeature : IServiceProvidersFeature
    {
        public ServiceProvidersFeature(IServiceProvider requestServices)
        {
            RequestServices = requestServices;
        }

        public IServiceProvider RequestServices { get; set; }
    }
}
