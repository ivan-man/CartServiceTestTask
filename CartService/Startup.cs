using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartService.Settings;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.DAL;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Common;
using Common.Interfaces;

namespace CartService
{
    public class Startup
    {
        private readonly AppSettings _appSettings = new AppSettings();

        public Startup()
        {
            Configuration = _appSettings.Configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_appSettings);
            services.AddSingleton<ICartServiceRepository>(new CartServiceRepository(_appSettings.CartServiceConnectionString));

            services.AddControllers();

            #region IdentityServer
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = _appSettings.AuthenticationSettings.AuthorityHost;
                    options.LegacyAudienceValidation = true;

                    options.ApiName = _appSettings.AuthenticationSettings.ApiName;
                    options.ApiSecret = _appSettings.AuthenticationSettings.ApiSecret;
                    options.RequireHttpsMetadata = false;
                });
            #endregion IdentityServer

#if DEBUG
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
            });
#else
                //ToDo Add any logger
#endif

            #region swagger
            services.AddSwaggerGen(q =>
            {
                q.SwaggerDoc("v1", new OpenApiInfo { Title = "CartService", Version = "0.1" });

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                q.IncludeXmlComments(commentsFile);
            });

            #endregion swagger
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart Service"));
        }
    }
}
