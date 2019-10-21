using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Data.Repositories;
using MyFunds.Library.Interfaces;
using MyFunds.Library.Services;

namespace MyFunds
{
    public class Startup
    {
        public IConfiguration Configuration { get; }





        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }





        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<MyFundsDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("MyFundsConnection"))
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging());

            services.AddIdentity<User, UserRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<MyFundsDbContext>();

            services.AddMvc();

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IFixedAssetRepository, FixedAssetRepository>();
            services.AddScoped<IFixedAssetArchiveRepository, FixedAssetArchiveRepository>();
            services.AddScoped<IMobileAssetRepository, MobileAssetRepository>();
            services.AddScoped<IMobileAssetArchiveRepository, MobileAssetArchiveRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IFixedAssetService, FixedAssetService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
