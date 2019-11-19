using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MyFunds.Data.Models;
using MyFunds.Enumerators;
using MyFunds.Exceptions;
using MyFunds.Filters;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Pdf;

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
        readonly IRoomService roomService;
        readonly IBuildingService buildingService;
        readonly IConverterService converterService;
        readonly IMapper mapper;

        public AdminController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IRoomService roomService, IBuildingService buildingService, IConverterService converterService, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.roomService = roomService;
            this.buildingService = buildingService;
            this.converterService = converterService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Give an admin role to user
        /// </summary>
        /// <response code="200">Successfuly given permission to user</response>
        /// <response code="400">Unable to finish request likely due to validation error</response>
        [HttpPost]
        [Route("GiveUserAdminRole/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
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

        /// <summary>
        /// Remove admin role from user
        /// </summary>
        /// <response code="200">Successfuly removed permission from user</response>
        /// <response code="400">Unable to finish request likely due to validation error</response>
        [HttpPost]
        [Route("RemoveAdminRole/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
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

        /// <summary>
        /// Get user with all assets that belongs to him
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetUserWithAssets/{userId}")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetUserWithAssets(int userId)
        {
            return Ok(userService.GetUserWithAssets(userId));
        }

        /// <summary>
        /// Get all users with their assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetAllUsersWithAssets")]
        [ProducesResponseType(typeof(List<UserDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllUsersWithAssets()
        {
            return Ok(userService.GetAllUsersWithAssets());
        }

        /// <summary>
        /// Get all fixed assets with its archives
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetFixedAssetWithArchives/{fixedAssetId}")]
        [ProducesResponseType(typeof(FixedAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetFixedAssetWithArchives(int fixedAssetId)
        {
            return Ok(fixedAssetService.GetFixedAssetWithArchives(fixedAssetId));
        }

        /// <summary>
        /// Get all mobile assets with its archives
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetMobileAssetWithArchives/{mobileAssetId}")]
        [ProducesResponseType(typeof(MobileAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetMobileAssetWithArchives(int mobileAssetId)
        {
            return Ok(mobileAssetService.GetMobileAssetWithArchives(mobileAssetId));
        }

        /// <summary>
        /// Get all rooms with its assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetRoomWithAssets/{roomId}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetRoomWithAssets(int roomId)
        {
            return Ok(roomService.GetRoomWithAssets(roomId));
        }

        /// <summary>
        /// Get all building with its assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetBuildingWithAssets/{buildingId}")]
        [ProducesResponseType(typeof(BuildingDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetBuildingWithAssets(int buildingId)
        {
            return Ok(buildingService.GetBuildingWithAssets(buildingId));
        }

        //TODO: make new sheet for every array in json string or handle somehow jArrays
        /// <summary>
        /// Export excel from given json
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("ExportExcel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> ExportExcel([FromQuery] string fileName = "file", string sheetName = "Sheet1")
        {
            var streamReader = new StreamReader(HttpContext.Request.Body);
            var requestBody = streamReader.ReadToEnd();

            var memoryStream = converterService.ConvertJsonToExcel(requestBody, sheetName);

            
            HttpContext.Response.Headers["Content-Disposition"] =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName + ".xlsx"
                }.ToString();

            HttpContext.Response.ContentLength = memoryStream.Length;
            await HttpContext.Response.Body.WriteAsync(memoryStream.ToArray());

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

    }
}