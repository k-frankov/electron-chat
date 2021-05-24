using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ElectronChatAPI.Models;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Westwind.AspNetCore.Security;

namespace ElectronChatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly IUserRepository userRepository;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.configuration = configuration;
            this.memoryCache = memoryCache;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserDto user)
        {
            try
            {
                UserEntity userByUserName = await this.TryFromCacheAddToCache(user);
                if (userByUserName == null)
                {
                    return NotFound("User not found - register first");
                }

                var passwordHasher = new PasswordHasher<UserDto>();
                PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, userByUserName.Password, user.Password);
                if (result != PasswordVerificationResult.Success)
                {
                    return Unauthorized("Wrong password.");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userByUserName.UserName)
                };

                var token = JwtHelper.GetJwtTokenString(
                    user.UserName,
                    this.configuration.GetValue<string>("JwtKey"),
                    this.configuration.GetValue<string>("JwtIssuer"),
                    this.configuration.GetValue<string>("JwtIssuer"),
                    TimeSpan.FromDays(30),
                    claims.ToArray());


                // TODO: Add automapper
                return Ok(new UserDto { UserName = userByUserName.UserName, JwtToken = token });
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(UserDto user)
        {
            try
            {
                UserEntity userByUserName = await this.TryFromCacheAddToCache(user);
                if (userByUserName != null)
                {
                    return BadRequest("User with same name already exists.");
                }

                var passwordHasher = new PasswordHasher<UserDto>();
                UserEntity userEntity = new UserEntity
                {
                    UserName = user.UserName,
                    Password = passwordHasher.HashPassword(user, user.Password),
                };

                //TODO: Add automapper 
                UserEntity newUser = await this.userRepository.CreateUserAsync(userEntity);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newUser.UserName)
                };

                var token = JwtHelper.GetJwtTokenString(
                    user.UserName,
                    this.configuration.GetValue<string>("JwtKey"),
                    this.configuration.GetValue<string>("JwtIssuer"),
                    this.configuration.GetValue<string>("JwtIssuer"),
                    TimeSpan.FromDays(30),
                    claims.ToArray());

                var newUserDto = new UserDto { UserName = newUser.UserName, JwtToken = token };
                return Ok(newUserDto);
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error");
            }
        }

        private async Task<UserEntity> TryFromCacheAddToCache(UserDto userDto)
        {
            UserEntity user = null;
            if (!this.memoryCache.TryGetValue(userDto.UserName, out user))
            {
                user = await this.userRepository.GetUserByUserNameAsync(userDto.UserName);
                if (user != null)
                {
                    this.memoryCache.Set(user.UserName, user,
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMonths(2),
                            SlidingExpiration = TimeSpan.FromDays(30),
                        });
                }
            }

            return user;
        }
    }
}
