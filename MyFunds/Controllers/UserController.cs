using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFunds.Filters;
using MyFunds.Library.Interfaces;

namespace MyFunds.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;


        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("GetUser/{userId}")]
        public IActionResult GetUser(int userId)
        {
            return Ok(userService.GetUser(userId));
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers(int userId)
        {
            return Ok(userService.GetAllUsers());
        }
    }
}