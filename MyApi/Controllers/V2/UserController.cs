using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Controllers.V1;
using MyApi.Models;
using Services;
using WebFramework;

namespace MyApi.Controllers.V2
{
    [ApiVersion("2")]
    public class UserController : V1.UserController
    {
        public UserController(IUserRepository userRepository, ILogger<V1.UserController> logger, IJwtService jwtService, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager) : base(userRepository, logger, jwtService, userManager, roleManager, signInManager)
        {
        }

        public override Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            return base.Create(userDto, cancellationToken);
        }

        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ActionResult<List<User>>> Get(CancellationToken cancelationtken)
        {
            return base.Get(cancelationtken);
        }

        public override Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        public override Task<ActionResult> Token(CancellationToken cancellationToken, [FromHeader] TokenRequest tokenRequest)
        {
            return base.Token(cancellationToken, tokenRequest);
        }

        public override Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            return base.Update(id, user, cancellationToken);
        }
    }
}