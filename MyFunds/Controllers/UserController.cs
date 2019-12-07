using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyFunds.Data.Models;
using MyFunds.Enumerators;
using MyFunds.Exceptions;
using MyFunds.Filters;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using MyFunds.ViewModels;

namespace MyFunds.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.User)]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;
        readonly UserManager<User> userManager;
        readonly IFixedAssetService fixedAssetService;
        readonly IMobileAssetService mobileAssetService;
        readonly IRoomService roomService;
        readonly IBuildingService buildingService;
        readonly LinkGenerator linkGenerator;
        readonly IMapper mapper;

        public UserController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IRoomService roomService, IBuildingService buildingService, LinkGenerator linkGenerator, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.roomService = roomService;
            this.buildingService = buildingService;
            this.linkGenerator = linkGenerator;
            this.mapper = mapper;
        }


        /// <summary>
        /// Get currently logged in user
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetMe")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetMe()
        {
            var id = userManager.GetUserId(User);

            int userId;
            if (!int.TryParse(id, out userId))
                throw new ApiException("Incorrect user Id");

            var user = userService.GetMe(userId, User);

            return Ok(user);
        }

        /// <summary>
        /// Get currently logged in user with his assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetMeWithAssets")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetMeWithAssets()
        {
            var id = userManager.GetUserId(User);

            int userId;
            if (!int.TryParse(id, out userId))
                throw new ApiException("Incorrect user Id");

            var user = userService.GetMeWithAssets(userId, User);

            return Ok(user);
        }


    }
}