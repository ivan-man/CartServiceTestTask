using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Model;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("identity")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            string role = "Basic";

            if (result.Succeeded)
            {
                if (await _roleManager.FindByNameAsync(role) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("firstName", user.FirstName));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("lastName", user.LastName));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", role));

                return Ok(new ProfileViewModel(user));
            }

            return BadRequest(result.Errors);
        }
    }
}
