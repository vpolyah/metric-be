using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Tele2_Metrics.Models;

namespace Tele2_Metrics
{
    public class Startup
    {
        public IConfigurationRoot AppConfiguration { get; set; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .AllowAnyHeader();
            }));
            services.AddAuthorization();
            services.AddMemoryCache();
            services.AddMvc().
                AddJsonOptions(opt => opt.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });
            //services.AddHealthChecks(checks =>
            //{
            //    checks.AddValueTaskCheck("HTTP Endpoint", () => new
            //    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            //});
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Tele2_Metrics",
                    Description = "Сервис метрик",
                    //License = new License
                    //{
                    //    Name = "Требование 17680: Сервис персональных данных",
                    //    Url = "https://tfs.tele2.ru/tfs/Main/Tele2/CRM%20Team%20B/_workitems?id=17680&_a=edit"
                    //}
                });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var basePath = Path.Combine(AppContext.BaseDirectory);
                var xmlPath = Path.Combine(basePath, "Tele2_Metrics.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.Configure<ConfigurationModel>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tele2_Metrics");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
        }
    }
}
