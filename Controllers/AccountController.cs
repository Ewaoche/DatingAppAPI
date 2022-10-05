using FinalDatingApp.Data;
using FinalDatingApp.DTOs;
using FinalDatingApp.Entities;
using FinalDatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FinalDatingApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController:ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task <ActionResult<AppUser>> RegisterUser(RegisterDTO registerModel)
        {
            // if (await UserExists(register.Username)) return BadRequest("Username is Taken");
            if (await UserExists(registerModel.Username))
            {
                return BadRequest("Username is taken");
            }
            using var hmac =  new HMACSHA512();
            var user = new AppUser()
            {
                UserName = registerModel.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerModel.Password)),
                PasswordSalt = hmac.Key
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
            
        }
         

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginModel)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x=> x.UserName == loginModel.Username);
            if (user == null) return Unauthorized("Invalid credentials");
           using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginModel.Password));

            for (int i=0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }



        private async Task<bool> UserExists(string username)
        {
            return await _dbContext.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }

    
}
