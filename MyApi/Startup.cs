using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Data;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyApi.Models;
using Services;
using Services.DataInitializer;
using Swashbuckle.AspNetCore.Swagger;
using WebFramework;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Swagger;

namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AutoMapperConfiguration.InitializeAutoMapper();

            //Mapper.Initialize(config =>
            //{
            //    config.CreateMap<Post, PostDto>().ReverseMap()
            //        .ForMember(p => p.Author, opt => opt.Ignore())
            //        .ForMember(p => p.Category, opt => opt.Ignore());
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.AddDbContext(Configuration);

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddMinimalMvc();


            services.AddElmah();

            services.AddSwagger();

            services.AddCustomApiVersioning();


            return services.BuildAutofacServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {



            app.IntializeDatabase();


            app.UseCustomExceptionHandler();


            app.UseHsts(env);
            app.UseElmah();
            app.UseHttpsRedirection();

            app.UseSwaggerAndUI();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
