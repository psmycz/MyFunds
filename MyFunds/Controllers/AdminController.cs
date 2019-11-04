using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFunds.Data.Models;
using MyFunds.Enumerators;
using MyFunds.Exceptions;
using MyFunds.Filters;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;

namespace MyFunds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.Admin)]
    public class AdminController : ControllerBase
    {
        readonly IUserService userService;
        readonly UserManager<User> userManager;
        readonly IFixedAssetService fixedAssetService;
        readonly IMobileAssetService mobileAssetService;
        readonly IMapper mapper;

        public AdminController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.mapper = mapper;
        }


        [HttpPost]
        [Route("GiveUserAdminRole/{userId}")]
        public async Task<IActionResult> GiveUserAdminRole(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                ModelState.AddModelError(nameof(userId), "User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            if(userClaims.Any(claim => claim.Value == Role.Admin))
            {
                ModelState.AddModelError(nameof(user), "User with provided Id already have Admin role");
                return new ValidationProblemDetailsResult();
            }

            var addClaimsResult = await userManager.AddClaimAsync(user, new Claim("role", Role.Admin));
            
            return addClaimsResult.Succeeded ? Ok() : throw new ApiException("An error occured while adding claims to user");
        }

        [HttpPost]
        [Route("RemoveAdminRole/{userId}")]
        public async Task<IActionResult> RemoveAdminRole(int userId)
        {
            int currentUserId;
           
            if(!int.TryParse(userManager.GetUserId(User), out currentUserId))
                throw new ApiException("An error occured accessing user data from token");
            if (currentUserId == userId)
            {
                ModelState.AddModelError(nameof(userId), "Cannot remove Admin role from yourself");
                return new ValidationProblemDetailsResult();
            }

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                ModelState.AddModelError(nameof(userId), "User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            if (!userClaims.Any(claim => claim.Value == Role.Admin))
            {
                ModelState.AddModelError(nameof(userId), "User with provided Id does not have Admin role");
                return new ValidationProblemDetailsResult();
            }

            var addClaimsResult = await userManager.RemoveClaimAsync(user, new Claim("role", Role.Admin));

            return addClaimsResult.Succeeded ? Ok() : throw new ApiException("An error occured while removing claims from user");
        }


        [HttpGet]
        [Route("GetUserWithAssets/{userId}")]
        public IActionResult GetUserWithAssets(int userId)
        {
            return Ok(userService.GetUserWithAssets(userId));
        }

        [HttpGet]
        [Route("GetAllUsersWithAssets")]
        public IActionResult GetAllUsersWithAssets()
        {
            return Ok(userService.GetAllUsersWithAssets());
        }

        [HttpGet]
        [Route("GetFixedAssetWithArchives/{fixedAssetId}")]
        public IActionResult GetFixedAssetWithArchives(int fixedAssetId)
        {
            throw new NotImplementedException();
            //return Ok(fixedAssetService.GetFixedAssetWithArchives(fixedAssetId));
        }

        [HttpGet]
        [Route("GetMobileAssetWithArchives/{mobileAssetId}")]
        public IActionResult GetMobileAssetWithArchives(int mobileAssetId)
        {
            throw new NotImplementedException();
            //return Ok(mobileAssetService.GetMobileAssetWithArchives(mobileAssetId));
        }

    }
}