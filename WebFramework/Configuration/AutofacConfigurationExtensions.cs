using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Data;
using Entities.Common;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Configuration
{
    public static class AutofacConfigurationExtensions
    {


        public static void AddServices(this ContainerBuilder containerBuilder)
        {

            //RegisterType > As > Liftetime



            var commonAssembly = typeof(SiteSettings).Assembly;
            var entitiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(JwtService).Assembly;

            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IJwtService, JwtService>();
            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            //containerBuilder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<JwtService>().As<IJwtService>().InstancePerLifetimeScope();


            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<IScopedDependency>
                ().AsImplementedInterfaces().
                InstancePerLifetimeScope();


            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ITransientDependency>
                ().AsImplementedInterfaces().
                InstancePerDependency();


            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
              .AssignableTo<ISingletonDependency>
              ().AsImplementedInterfaces().
              SingleInstance();

        }







        public static IServiceProvider BuildAutofacServiceProvider(this IServiceCollection services)
        {
            //var containerBuilder = new ContainerBuilder();
            ////register type >as >lifetime

            //containerBuilder.AddServices();
            //containerBuilder.Populate(services);
            //var container = containerBuilder.Build();

            //return new AutofacServiceProvider(container);




            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            //Register Services to Autofac ContainerBuilder
            containerBuilder.AddServices();

            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }
    }
}
