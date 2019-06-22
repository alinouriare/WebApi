using Common;
using Data;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Configuration
{
    public static class IdentityConfigurationExtensions
    {

        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings setting)
        {

            services.AddIdentity<User, Role>(identityOptions =>
            {
                identityOptions.Password.RequireDigit = setting.PasswordRequireDigit;
                identityOptions.Password.RequiredLength = setting.PasswordRequiredLength;
                identityOptions.Password.RequireNonAlphanumeric = setting.PasswordRequireNonAlphanumic; //#@!
                identityOptions.Password.RequireUppercase = setting.PasswordRequireUppercase;
                identityOptions.Password.RequireLowercase = setting.PasswordRequireLowercase;

                //UserName Settings
                identityOptions.User.RequireUniqueEmail = setting.RequireUniqueEmail;

                //Singin Settings
                //identityOptions.SignIn.RequireConfirmedEmail = false;
                //identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

                //Lockout Settings
                //identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                //identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                //identityOptions.Lockout.AllowedForNewUsers = false;


            }
            ).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
