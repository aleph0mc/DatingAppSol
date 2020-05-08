using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //if this is removed to validate the model we need to use ModelState
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _Config;

        private readonly IAuthRepository _Repo;

        public AuthController(IAuthRepository Repo, IConfiguration Config)
        {
            _Repo = Repo;
            _Config = Config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto UsrForRegisterDto)
        {
            //validate request
            //if [ApiController] is removed we need to valida the model against the ModelState
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            UsrForRegisterDto.Username = UsrForRegisterDto.Username.ToLower();

            if (await _Repo.UserExists(UsrForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = UsrForRegisterDto.Username,
            };

            var createdUser = await _Repo.Register(userToCreate, UsrForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto UsrForLoginDto)
        {
            var userFromRepo = await _Repo.Login(UsrForLoginDto.Username.ToLower(), UsrForLoginDto.Password);

            if (null == userFromRepo)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

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

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}