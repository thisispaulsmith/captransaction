using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CapTrans
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddControllers();
            services.AddDbContext<MyDbContext>(o => {
                o.UseSqlServer(_config["ConnectionStrings:MyConnectionString"]);
            });

            services.AddCap(x =>
            {
                x.UseSqlServer(_config["ConnectionStrings:MyConnectionString"]);
                x.UseRabbitMQ(c =>
                {
                    c.HostName = "localhost";
                    c.UserName = "guest";
                    c.Password = "guest";
                    c.Port = 5672;
                });
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
