using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Data;
using ElmahCore;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApi.Models;
using Services;
using WebFramework;
using WebFramework.Api;

namespace MyApi.Controllers.V1
{
    [ApiVersion("1")]
    public class UserController : BaseController
    {
        private readonly IUserRepository userRepository;

        private readonly ILogger<UserController> logger;

        private readonly IJwtService jwtService;

        private readonly UserManager<User> userManager;

        private readonly RoleManager<Role> roleManager;

        private readonly SignInManager<User> signInManager;
        public UserController(IUserRepository userRepository, ILogger<UserController> logger,
            IJwtService jwtService, UserManager<User> userManager, RoleManager<Role> roleManager
            ,SignInManager<User> signInManager)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.jwtService = jwtService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]

        public virtual async Task<ActionResult<List<User>>> Get(CancellationToken cancelationtken)
        {

            var userName = HttpContext.User.Identity.GetUserName();
            var userId = HttpContext.User.Identity.GetUserId();
            var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await userRepository.TableNoTracking.ToListAsync(cancelationtken);
            return users;

        }



        [HttpGet("{id:int}")]

    
        public virtual async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user2 = await userManager.FindByIdAsync(id.ToString());
            var rolea = await roleManager.FindByNameAsync("Admin");

            var user = await userRepository.GetByIdAsync(cancellationToken, id);


            //await userManager.UpdateSecurityStampAsync(user);
            if (user == null)
                return NotFound();

            //await userRepository.UpdateSecuirtyStampAsync(user, cancellationToken);

            return user;
        }
        [HttpPost]
      
        public virtual async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            //var exsist = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
            //if (exsist)
            //    return BadRequest("نام کاربر تکراری هست !");


            logger.LogError("متد Create فراخوانی شد");
            HttpContext.RiseError(new Exception("متد Create فراخوانی شد"));

            var user = new User
            {

                Age = userDto.Age,
                FullName = userDto.FullName,
                Gender = userDto.Gender,
                UserName = userDto.UserName,
                Email=userDto.Email
                
            };

            var result1 = await userManager.CreateAsync(user, userDto.Password);
            var result2 = await roleManager.CreateAsync(new Role {

                Name="Admin",
                Description= "admin role"
            });

            var result3 = await userManager.AddToRoleAsync(user, "Admin");

            //await userRepository.AddAsync(user, userDto.Password, cancellationToken);

            return user;
        }

        //[HttpPut]
        [HttpPut("{id:int}")]
        public virtual async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = user.UserName;
            updateUser.PasswordHash = user.PasswordHash;
            updateUser.FullName = user.FullName;
            updateUser.Age = user.Age;
            updateUser.Gender = user.Gender;
            updateUser.IsActive = user.IsActive;
            updateUser.LastLoginDate = user.LastLoginDate;

            await userRepository.UpdateAsync(updateUser, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {

            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            await userRepository.DeleteAsync(user, cancellationToken);
            return Ok();

        }

        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Token(


        //    [FromQuery] string userName, string password, 

        //    ,string grant_type,string client_id,string client_secret

        CancellationToken cancellationToken ,[FromForm]TokenRequest tokenRequest
            )


        {

            if (!tokenRequest.grant_type.Equals("password",StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            //var ddd =userName ?? Request.Form[nameof(userName)].ToString();
            //var pass = password ?? Request.Form[nameof(password)].ToString();


            //var user = await userRepository.GetByUserAndPass(userName, password, cancellationToken);

            var user = await userManager.FindByNameAsync(tokenRequest.username);

            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه  است!");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, tokenRequest.password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه  است!");

            var jwt =await jwtService.GenerateAsync(user);


            return new JsonResult(jwt);
        }



    }
}