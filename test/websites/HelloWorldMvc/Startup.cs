
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;

namespace HelloWorldMvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvcWithDefaultRoute();
        }
    }
}
