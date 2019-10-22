using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFunds.Data.Models;
using MyFunds.Exceptions;
using MyFunds.Filters;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using MyFunds.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AccountController : ControllerBase
    {
        readonly UserManager<User> userManager;

        public AccountController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (await IsUsernameInUse(registerRequest.UserName))
            {
                ModelState.AddModelError("Username", "Username is already in use");
                return new ValidationProblemDetailsResult();
            }
            if (await IsEmailInUse(registerRequest.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use");
                return new ValidationProblemDetailsResult();
            }

            var user = new User
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email
            };


            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return new ValidationProblemDetailsResult();
            }
        }

        // TODO: check if token work after removing user
        [HttpPost]
        [Route("Remove")]
        public async Task<IActionResult> Remove()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null) 
                throw new ApiException("An error occured while accessing user data");

            var result = await userManager.DeleteAsync(currentUser);
            
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return new ValidationProblemDetailsResult();
            }
        }


        [AllowAnonymous]
        async Task<bool> IsUsernameInUse(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            return user != null;
        }

        [AllowAnonymous]
        async Task<bool> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user != null;
        }

    }
}
