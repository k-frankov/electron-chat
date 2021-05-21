using System;
using System.Threading.Tasks;
using ElectronChatAPI.Models;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectronChatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly IUserRepository userRepository;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(UserDto user)
        {
            try
            {
                UserEntity userByUserName = await this.userRepository.GetUserByUserName(user.UserName);
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

                // TODO: Add automapper
                return Ok(new UserDto { UserName = userByUserName.UserName});
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
                UserEntity userByUserName = await this.userRepository.GetUserByUserName(user.UserName);
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
                UserEntity newUser = await this.userRepository.CreateUser(userEntity);
                return Ok(new UserDto { UserName = newUser.UserName });
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error");
            }
        }
    }
}
