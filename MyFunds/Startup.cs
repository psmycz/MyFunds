using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Data.Repositories;
using MyFunds.Exceptions;
using MyFunds.Extensions;
using MyFunds.Filters;
using MyFunds.Library.Interfaces;
using MyFunds.Library.MappingProfiles;
using MyFunds.Library.Services;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

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
            

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["IdentityServerAddress"];
                    options.RequireHttpsMetadata = false;

                    options.Audience = "MyFundsApi";
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            const string claimType = ClaimTypes.Role; // => "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"; 
                            if (!context.Principal.HasClaim(c => c.Type == claimType))
                            {
                                context.Fail($"The claim '{claimType}' is not present in the token.");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });


            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);


            // controller with [ApiController] attribute does auto validation for ModelState, here custom error response
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context => new ValidationProblemDetailsResult();
            });

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IFixedAssetRepository, FixedAssetRepository>();
            services.AddScoped<IFixedAssetArchiveRepository, FixedAssetArchiveRepository>();
            services.AddScoped<IMobileAssetRepository, MobileAssetRepository>();
            services.AddScoped<IMobileAssetArchiveRepository, MobileAssetArchiveRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IFixedAssetService, FixedAssetService>();
            services.AddScoped<IFixedAssetArchiveService, FixedAssetArchiveService>();
            services.AddScoped<IMobileAssetService, MobileAssetService>();
            services.AddScoped<IMobileAssetArchiveService, MobileAssetArchiveService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoomService, RoomService>();

            services.AddScoped<ApiExceptionFilter>();


            // need assembly where namespace MappingProfiles is declared and its declared in different project so need that .dll file
            // TODO: change if found better way to do this
            // will be necessary to add other project assembly's when creating profiles in them
            services.AddAutoMapper(Assembly.GetEntryAssembly(), Assembly.LoadFrom(Assembly.GetEntryAssembly().Location.Replace("MyFunds.dll", "MyFunds.Library.dll")));



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API Instruction",
                });
                c.CustomSchemaIds(i => i.FullName);
                var basePath = PlatformServices
                                    .Default
                                    .Application
                                    .ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Swagger.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }





        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(context => 
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Instance = $"urn:MyFunds:error:{Guid.NewGuid()}"
                    };
                    if (exception is BadHttpRequestException badHttpRequestException)
                    {
                        problemDetails.Title = "Invalid Operation";
                        problemDetails.Status = badHttpRequestException.StatusCode;
                        problemDetails.Detail = badHttpRequestException.Message;
                    }
                    else if (exception.GetBaseException() is SocketException socketException)
                    {
                        problemDetails.Title = "Server Error";
                        problemDetails.Status = 500;
                        problemDetails.Detail = $"{socketException.Message} - The instance value should be used to identify the problem when calling customer support";
                    }
                    else 
                    {
                        problemDetails.Title = "An unexpected error occurred!";
                        problemDetails.Status = 500;
                        problemDetails.Detail = "The instance value should be used to identify the problem when calling customer support";
                    }
                    
                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.WriteJson(problemDetails, "application/problem+json");
                    
                    //TODO: add logger
                    /*
                    _logger.LogError($"\nTitle: {problemDetails.Title}" +
                        $"\nStatus: {problemDetails.Status}" +
                        $"\nInstance: {problemDetails.Instance}" +
                        $"\nDetails: {problemDetails.Detail}" +
                        $"\nStackTrace: {exception.Demystify().ToString()}" +
                        $"\nOriginalException: {exception.ToString()}");

                    */

                    // pls fix someone if could
                    return Task.CompletedTask;
                });
            });

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var swaggerPath = "/swagger/v1/swagger.json";
                c.SwaggerEndpoint(swaggerPath, "MyFunds API V1");
            });
        }
    }
}
