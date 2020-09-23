using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //if this is removed to validate the model we need to use ModelState
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Config;
        private readonly IMapper _Mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration Config, IMapper Mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _Config = Config;
            _Mapper = Mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config.GetSection("AppSettings:Token").Value));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UsrForRegisterDto)
        {
            //validate request
            //if [ApiController] is removed we need to validate the model against the ModelState
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            var userToCreate = _Mapper.Map<User>(UsrForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, UsrForRegisterDto.Password);

            var userToReturn = _Mapper.Map<UserForDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    id = userToCreate.Id
                }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto UsrForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(UsrForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, UsrForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = _Mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = await GenerateJwtToken(user),
                    user = appUser
                });
            }

            return Unauthorized();
        }
    }
}