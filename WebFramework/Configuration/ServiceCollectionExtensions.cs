﻿using Common;
using Common.Utilities;
using Data;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
        }

        public static void AddMinimalMvc(this IServiceCollection services)
        {
            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter());
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMvcCore(options =>
            {
                options.Filters.Add(new AuthorizeFilter());

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                //options.UseYeKeModelBinder();
            }



                ).AddApiExplorer().AddAuthorization().AddFormatterMappings().AddDataAnnotations().AddJsonFormatters(/*options =>
            {
                options.Formatting = Newtonsoft.Json.Formatting.Indented;
                options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            }*/).AddCors().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
        }

        public static void AddElmah(this IServiceCollection services, IConfiguration configuration, SiteSettings siteSetting)
        {
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.Path = siteSetting.ElmahPath;
                options.ConnectionString = configuration.GetConnectionString("Elmah");
                //options.CheckPermissionAction = httpContext =>
                //{
                //    return httpContext.User.Identity.IsAuthenticated;
                //};
            });
        }
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings settings)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(settings.SecretKey);
                var encryptionkey = Encoding.UTF8.GetBytes(settings.Encryptkey);

                var validationParameters = new TokenValidationParameters
                {

                    ClockSkew = TimeSpan.Zero, //default :5 min
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,//default :false
                    ValidAudience = settings.Audience,

                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)

                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        if (context.Exception != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, null);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                     {
                         //var applicationSignInManager = context.HttpContext.RequestServices.GetRequiredService<IApplicationSignInManager>();
                         var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                         var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                         var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                         if (claimsIdentity.Claims?.Any() != true)
                             context.Fail("This Token has no Claim.");

                         var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                         if (!securityStamp.HasValue())
                             context.Fail("This Token has no Security Stamp.");

                         //Find user and token from database and perform your custom validation
                         var userId = claimsIdentity.GetUserId<int>();
                         var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                         //if (user.SecurityStamp !=Guid.Parse(securityStamp))
                         //    context.Fail("Token secuirty stamp is not valid.");
                         var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                         if (validatedUser == null)
                             context.Fail("Token secuirty stamp is not valid.");


                         if (!user.IsActive)
                             context.Fail("User is not active.");

                         await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                     },
                    OnChallenge = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);


                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                        //return Task.CompletedTask;
                    }

                };
            });
        }


        public static void AddCustomApiVersioning(this IServiceCollection services)
        {

            services.AddApiVersioning(option =>

            {
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);//=>v1.0 ==v1
                option.ReportApiVersions = true;

                //ApiVersion.TryParse("1.0", out var version01);
                //ApiVersion.TryParse("1", out var version011);
                //var a = version01 == version011;



                //    //option.ApiVersionReader = new QueryStringApiVersionReader("api-version");

                //    // api/posts?api-version=1

                //    //options.ApiVersionReader = new UrlSegmentApiVersionReader();
                //    // api/v1/posts

                //    //options.ApiVersionReader = new HeaderApiVersionReader(new[] { "Api-Version" });
                //    // header => Api-Version : 1

                //    //options.ApiVersionReader = new MediaTypeApiVersionReader()

                //    //options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"), new UrlSegmentApiVersionReader())
                //    // combine of [querystring] & [urlsegment]

            });
        }


    }
}
