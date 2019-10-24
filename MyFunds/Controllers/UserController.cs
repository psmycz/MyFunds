using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFunds.Data.Models;
using MyFunds.Filters;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;

namespace MyFunds.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;
        readonly UserManager<User> userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        // admin only?
        [HttpGet]
        [Route("GetUser/{userId}")]
        public IActionResult GetUser(int userId)
        {
            return Ok(userService.GetUser(userId));
        }

        // admin only?
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers(int userId)
        {
            return Ok(userService.GetAllUsers());
        }

        [HttpGet]
        [Route("GetMe")]
        public IActionResult GetMe()
        {
            var id = userManager.GetUserId(User);
            
            int userId;
            if (!int.TryParse(id, out userId))
                throw new ApiException("Incorrect user Id");

            var user = userService.GetUser(userId);

            return Ok(user);
        }


    }
}