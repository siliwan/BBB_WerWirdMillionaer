using Backend.Data.Models;
using Backend.Data.Models.Auth;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private const string USER_ID_CLAIM_TYPE = "USER_ID";

        private readonly ILogger<LoginController> logger;
        private readonly IUserRepository userRepository;

        public LoginController(ILogger<LoginController> logger,
                               IUserRepository userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        [HttpPost]
        [Authorize]
        [Route(nameof(Logout))]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [Authorize]
        [Route(nameof(LoginInformation))]
        public ActionResult<SimpleUser> LoginInformation()
        {
            var UserIdString = User.Claims.Where(x => x.Type == USER_ID_CLAIM_TYPE)
                                          .SingleOrDefault()
                                          .Value;
            if (!int.TryParse(UserIdString, out int UserId))
            {
                return Unauthorized();
            }

            var RegisteredUser = userRepository.GetById(UserId);
            return new SimpleUser
            {
                Id = RegisteredUser.Id,
                Username = RegisteredUser.Username
            };
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            if(!TryValidateModel(loginModel))
            {
                return BadRequest();
            }
            
            if(!userRepository.Login(loginModel.Username, loginModel.Password, out User user))
            {
                return Unauthorized("Wrong Username/Password combination given!");
            }

            var claims = new List<Claim>
            {
                new Claim(USER_ID_CLAIM_TYPE, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(identity));
            return Ok(new SimpleUser
            {
                Id = user.Id,
                Username = user.Username
            });
        }
    }
}
