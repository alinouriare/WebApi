using Common;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SiteSettings _siteSetting;

        private readonly SignInManager<User> signInManager;
        public JwtService(IOptionsSnapshot<SiteSettings> settings, SignInManager<User> signInManager)
        {
            _siteSetting = settings.Value;
            this.signInManager = signInManager;
        }
        public async Task<AccessToken> GenerateAsync(User user)
        {
            //متن کلید آرایه بایتیشو گرفتیم
            //16 کارکتر باشد "My Secret key"
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);


            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Encryptkey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
            var claims = await _getClaimsAsync(user);










            var descriptor = new SecurityTokenDescriptor
            {

                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)

            };

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor) ;


            //var jwt = tokenHandler.WriteToken(securityToken);


            return new AccessToken(securityToken);


        }



        private async Task<IEnumerable<Claim>> _getClaimsAsync(User user)
        {


            var result = await signInManager.ClaimsFactory.CreateAsync(user);


            var list = new List<Claim>(result.Claims);

            list.Add(new Claim(ClaimTypes.MobilePhone, "09359504672"));

            return list;
            //add custom claimsv

            //return result.Claims;


            ////JwtRegisteredClaimNames                     claim غید .net برای خود jwt
            //var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
            //var list = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name,user.UserName),
            //    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            //    new Claim(ClaimTypes.MobilePhone,"09359504672"),
            //    new Claim(securityStampClaimType,user.SecurityStamp.ToString())

            //};
            //var roles = new Role[] { new Role { Name = "Admin" } };

            //foreach (var role in roles)
            //    list.Add(new Claim(ClaimTypes.Role, role.Name));

            //return list;

        }
    }
}
