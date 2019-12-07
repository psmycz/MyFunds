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
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using MyFunds.Data.Models;
using MyFunds.Enumerators;
using MyFunds.Exceptions;
using MyFunds.Filters;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using MyFunds.ViewModels;
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
        readonly LinkGenerator linkGenerator;
        readonly IMapper mapper;

        public AdminController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IRoomService roomService, IBuildingService buildingService, IConverterService converterService, LinkGenerator linkGenerator, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.roomService = roomService;
            this.buildingService = buildingService;
            this.converterService = converterService;
            this.linkGenerator = linkGenerator;
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
            if (userClaims.Any(claim => claim.Value == Role.Admin))
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

            if (!int.TryParse(userManager.GetUserId(User), out currentUserId))
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
        /// Get user with provided id
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetUser/{userId}")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(int userId)
        {
            return Ok(userService.GetUser(userId));
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
        /// Get all users
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllUsers")]
        [ProducesResponseType(typeof(List<UserDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllUsers()
        {
            return Ok(userService.GetAllUsers());
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
        /// Get fixed asset with provided id
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetFixedAsset/{fixedAssetId}")]
        [ProducesResponseType(typeof(FixedAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetFixedAsset(int fixedAssetId)
        {
            return Ok(fixedAssetService.GetFixedAsset(fixedAssetId));
        }
        /// <summary>
        /// Get fixed asset with its archives
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
        /// Get all fixed assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllFixedAssets")]
        [ProducesResponseType(typeof(List<FixedAssetDTO>), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllFixedAssets(string name, double? minPrice, double? maxPrice, bool? inUse, DateTime? maxPurchaseDate, DateTime? maxWarrantyEndDate, string fixedAssetType, string roomType)
        {
            var fixedAssets = fixedAssetService.GetAllFixedAssets();

            if (!string.IsNullOrEmpty(name)) fixedAssets = fixedAssets.Where(fa => fa.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (minPrice != null) fixedAssets = fixedAssets.Where(fa => fa.Price > minPrice).ToList();

            if (maxPrice != null) fixedAssets = fixedAssets.Where(fa => fa.Price < maxPrice).ToList();

            if (inUse != null) fixedAssets = fixedAssets.Where(fa => fa.InUse == inUse).ToList();

            if (maxPurchaseDate != null) fixedAssets = fixedAssets.Where(fa => DateTime.Compare(fa.PurchaseDate, maxPurchaseDate.GetValueOrDefault()) <= 0).ToList();

            if (maxWarrantyEndDate != null) fixedAssets = fixedAssets.Where(fa => DateTime.Compare(fa.WarrantyEndDate, maxWarrantyEndDate.GetValueOrDefault()) <= 0).ToList();

            if (!string.IsNullOrEmpty(fixedAssetType))
            {
                FixedAssetType type;

                bool success = Enum.TryParse<FixedAssetType>(fixedAssetType, true, out type);
                if (!success)
                {
                    ModelState.AddModelError(nameof(fixedAssetType), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(FixedAssetType)))}");
                    return new ValidationProblemDetailsResult();
                }
                else
                    fixedAssets = fixedAssets.Where(fa => fa.Type == type).ToList();
            }

            if (!string.IsNullOrEmpty(roomType))
            {
                RoomType type;

                bool success = Enum.TryParse<RoomType>(roomType, true, out type);
                if (!success)
                {
                    ModelState.AddModelError(nameof(roomType), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(RoomType)))}");
                    return new ValidationProblemDetailsResult();
                }
                else
                    fixedAssets = fixedAssets.Where(fa => fa.Room.Type == type).ToList();
            }

            return Ok(fixedAssets);
        }



        /// <summary>
        /// Get mobile asset with provided id
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetMobileAsset/{mobileAssetId}")]
        [ProducesResponseType(typeof(MobileAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetMobileAsset(int mobileAssetId)
        {
            return Ok(mobileAssetService.GetMobileAsset(mobileAssetId));
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
        /// Get all mobile assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllMobileAssets")]
        [ProducesResponseType(typeof(List<MobileAssetDTO>), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllMobileAssets(string name, double? minPrice, double? maxPrice, bool? inUse, DateTime? maxPurchaseDate, DateTime? maxWarrantyEndDate)
        {
            var mobileAssets = mobileAssetService.GetAllMobileAssets();

            if (!string.IsNullOrEmpty(name)) mobileAssets = mobileAssets.Where(ma => ma.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (minPrice != null) mobileAssets = mobileAssets.Where(ma => ma.Price > minPrice).ToList();

            if (maxPrice != null) mobileAssets = mobileAssets.Where(ma => ma.Price < maxPrice).ToList();

            if (inUse != null) mobileAssets = mobileAssets.Where(ma => ma.InUse == inUse).ToList();

            if (maxPurchaseDate != null) mobileAssets = mobileAssets.Where(ma => DateTime.Compare(ma.PurchaseDate, maxPurchaseDate.GetValueOrDefault()) <= 0).ToList();

            if (maxWarrantyEndDate != null) mobileAssets = mobileAssets.Where(ma => DateTime.Compare(ma.WarrantyEndDate, maxWarrantyEndDate.GetValueOrDefault()) <= 0).ToList();

            return Ok(mobileAssets);
        }

        /// <summary>
        /// Get room by provided id
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetRoom/{roomId}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetRoom(int roomId)
        {
            return Ok(roomService.GetRoom(roomId));
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
        /// Get all rooms
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllRooms")]
        [ProducesResponseType(typeof(List<RoomDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllRooms()
        {
            return Ok(roomService.GetAllRooms());
        }


        /// <summary>
        /// Get building by provided id
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetBuilding/{buildingId}")]
        [ProducesResponseType(typeof(BuildingDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetBuilding(int buildingId)
        {
            return Ok(buildingService.GetBuilding(buildingId));
        }
        /// <summary>
        /// Get building by provided id with all rooms inside it
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetBuildingWithRooms/{buildingId}")]
        [ProducesResponseType(typeof(BuildingDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetBuildingWithRooms(int buildingId)
        {
            return Ok(buildingService.GetBuildingWithRooms(buildingId));
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
        /// <summary>
        /// Get all buildings
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllBuildings")]
        [ProducesResponseType(typeof(List<BuildingDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllBuildings()
        {
            return Ok(buildingService.GetAllBuildings());
        }
        /// <summary>
        /// Get all buildings with its rooms
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        /// <response code="404">Not found any data to return</response>
        [HttpGet]
        [Route("GetAllBuildingsWithRooms")]
        [ProducesResponseType(typeof(List<BuildingDTO>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllBuildingsWithRooms()
        {
            return Ok(buildingService.GetAllBuildingsWithRooms());
        }



        /// <summary>
        /// Add new fixed asset
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("AddFixedAsset")]
        [ProducesResponseType(typeof(FixedAssetDTO), 201)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult AddFixedAsset(FixedAssetRequest fixedAsset)
        {
            if (fixedAsset.Type == null)
            {
                ModelState.AddModelError(nameof(fixedAsset.Type), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(FixedAssetType)))}");
                return new ValidationProblemDetailsResult();
            }
            if (DateTime.Compare(fixedAsset.PurchaseDate, fixedAsset.WarrantyEndDate) >= 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.PurchaseDate), $"Purchase Date must be earlier than Warranty End Date");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.InUse && (fixedAsset.Type != FixedAssetType.Rentable.ToString() || fixedAsset.UserId == null))
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
                return new ValidationProblemDetailsResult();
            }
            if (!fixedAsset.InUse && fixedAsset.UserId != null)
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.UserId != null && !userService.UserExist(fixedAsset.UserId.GetValueOrDefault()))
            {
                ModelState.AddModelError(nameof(fixedAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.RoomId == 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.RoomId), $"Field RoomId is required");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.RoomId != 0 && !roomService.RoomExist(fixedAsset.RoomId))
            {
                ModelState.AddModelError(nameof(fixedAsset.RoomId), $"Room with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }


            var fixedAssetDTO = mapper.Map<FixedAssetDTO>(fixedAsset);
            var newAsset = fixedAssetService.Create(fixedAssetDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetFixedAsset", "Admin", values: new { fixedAssetId = newAsset.Id });
            return Created(link, newAsset);
        }
        /// <summary>
        /// Update existing fixed asset
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("UpdateFixedAsset")]
        [ProducesResponseType(typeof(FixedAssetDTO), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult UpdateFixedAsset(FixedAssetRequest fixedAsset)
        {
            if (fixedAsset.Id == 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.Id), $"Id of an item to update is required");
                return new ValidationProblemDetailsResult();
            }
            if (!fixedAssetService.FixedAssetExist(fixedAsset.Id))
            {
                ModelState.AddModelError(nameof(fixedAsset.Id), $"Fixed asset with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.Type == null)
            {
                ModelState.AddModelError(nameof(fixedAsset.Type), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(FixedAssetType)))}");
                return new ValidationProblemDetailsResult();
            }
            if (DateTime.Compare(fixedAsset.PurchaseDate, fixedAsset.WarrantyEndDate) >= 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.PurchaseDate), $"Purchase Date must be earlier than Warranty End Date");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.InUse && (fixedAsset.Type != FixedAssetType.Rentable.ToString() || fixedAsset.UserId == null))
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
                return new ValidationProblemDetailsResult();
            }
            if (!fixedAsset.InUse && fixedAsset.UserId != null)
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.UserId != null && !userService.UserExist(fixedAsset.UserId.GetValueOrDefault()))
            {
                ModelState.AddModelError(nameof(fixedAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.RoomId == 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.RoomId), $"Field RoomId is required");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.RoomId != 0 && !roomService.RoomExist(fixedAsset.RoomId))
            {
                ModelState.AddModelError(nameof(fixedAsset.RoomId), $"Room with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }

            var fixedAssetDTO = mapper.Map<FixedAssetDTO>(fixedAsset);
            var newAsset = fixedAssetService.Update(fixedAssetDTO);

            return Ok(newAsset);
        }

        /// <summary>
        /// Add new mobile asset
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("AddMobileAsset")]
        [ProducesResponseType(typeof(MobileAssetDTO), 201)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult AddMobileAsset(MobileAssetRequest mobileAsset)
        {
            if (mobileAsset.InUse && mobileAsset.UserId == null)
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"Must provide UserId when InUse");
                return new ValidationProblemDetailsResult();
            }
            if (!mobileAsset.InUse && mobileAsset.UserId != null)
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (mobileAsset.UserId != null && !userService.UserExist(mobileAsset.UserId.GetValueOrDefault()))
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (DateTime.Compare(mobileAsset.PurchaseDate, mobileAsset.WarrantyEndDate) >= 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.PurchaseDate), $"Purchase Date must be earlier than Warranty End Date");
                return new ValidationProblemDetailsResult();
            }


            var mobileAssetDTO = mapper.Map<MobileAssetDTO>(mobileAsset);
            var newAsset = mobileAssetService.Create(mobileAssetDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetMobileAsset", "Admin", values: new { mobileAssetId = newAsset.Id });
            return Created(link, newAsset);
        }
        /// <summary>
        /// Update existing mobile asset
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("UpdateMobileAsset")]
        [ProducesResponseType(typeof(MobileAssetDTO), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult UpdateMobileAsset(MobileAssetRequest mobileAsset)
        {
            if (mobileAsset.Id == 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.Id), $"Id of an item to update is required");
                return new ValidationProblemDetailsResult();
            }
            if (!mobileAssetService.MobileAssetExist(mobileAsset.Id))
            {
                ModelState.AddModelError(nameof(mobileAsset.Id), $"Mobile asset with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (DateTime.Compare(mobileAsset.PurchaseDate, mobileAsset.WarrantyEndDate) >= 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.PurchaseDate), $"Purchase Date must be earlier than Warranty End Date");
                return new ValidationProblemDetailsResult();
            }
            if (mobileAsset.InUse && mobileAsset.UserId == null)
            {
                ModelState.AddModelError(nameof(mobileAsset.InUse), $"UserId must be provided when InUse");
                return new ValidationProblemDetailsResult();
            }
            if (!mobileAsset.InUse && mobileAsset.UserId != null)
            {
                ModelState.AddModelError(nameof(mobileAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (mobileAsset.UserId != null && !userService.UserExist(mobileAsset.UserId.GetValueOrDefault()))
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }

            var mobileAssetDTO = mapper.Map<MobileAssetDTO>(mobileAsset);
            var newAsset = mobileAssetService.Update(mobileAssetDTO);

            return Ok(newAsset);
        }

        /// <summary>
        /// Add new room
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("AddRoom")]
        [ProducesResponseType(typeof(RoomDTO), 201)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult AddRoom(RoomRequest room)
        {
            if (room.Type == null)
            {
                ModelState.AddModelError(nameof(room.Type), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(RoomType)))}");
                return new ValidationProblemDetailsResult();
            }
            if (room.BuildingId != 0 && !buildingService.BuildingExist(room.BuildingId))
            {
                ModelState.AddModelError(nameof(room.BuildingId), $"Building with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }


            var roomDTO = mapper.Map<RoomDTO>(room);
            var newRoom = roomService.Create(roomDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetRoom", "Admin", values: new { roomId = newRoom.Id });
            return Created(link, newRoom);
        }
        /// <summary>
        /// Update existing room
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("UpdateRoom")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult UpdateRoom(RoomRequest room)
        {
            if (room.Id == 0)
            {
                ModelState.AddModelError(nameof(room.Id), $"Id of an item to update is required");
                return new ValidationProblemDetailsResult();
            }
            if (!roomService.RoomExist(room.Id))
            {
                ModelState.AddModelError(nameof(room.Id), $"Room with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (room.Type == null)
            {
                ModelState.AddModelError(nameof(room.Type), $"Possible types are: {string.Join(", ", Enum.GetNames(typeof(RoomType)))}");
                return new ValidationProblemDetailsResult();
            }
            if (room.BuildingId != 0 && !buildingService.BuildingExist(room.BuildingId))
            {
                ModelState.AddModelError(nameof(room.BuildingId), $"Building with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }


            var roomDTO = mapper.Map<RoomDTO>(room);
            var newRoom = roomService.Update(roomDTO);

            return Ok(newRoom);
        }

        /// <summary>
        /// Add new building
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("AddBuilding")]
        [ProducesResponseType(typeof(BuildingDTO), 201)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult AddBuilding(BuildingRequest building)
        {

            var buildingDTO = mapper.Map<BuildingDTO>(building);
            var newBuilding = buildingService.Create(buildingDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetBuilding", "Admin", values: new { buildingId = newBuilding.Id });
            return Created(link, newBuilding);
        }
        /// <summary>
        /// Update existing building
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("UpdateBuilding")]
        [ProducesResponseType(typeof(BuildingDTO), 200)]
        [ProducesResponseType(typeof(Exceptions.ValidationProblemDetails), 400)]
        public IActionResult UpdateBuilding(BuildingRequest building)
        {
            if (building.Id == 0)
            {
                ModelState.AddModelError(nameof(building.Id), $"Id of an item to update is required");
                return new ValidationProblemDetailsResult();
            }
            if (!buildingService.BuildingExist(building.Id))
            {
                ModelState.AddModelError(nameof(building.Id), $"Building with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }


            var buildingDTO = mapper.Map<BuildingDTO>(building);
            var newBuilding = buildingService.Update(buildingDTO);

            return Ok(newBuilding);
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

        [HttpPost]
        [Route("ExportPdf")]
        public async Task<IActionResult> ExportPdf(string html)
        {
            var rs = new LocalReporting()
                .UseBinary(JsReportBinary.GetBinary())
                .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                .AsUtility()
                .Create();

            var report = await rs.RenderAsync(new RenderRequest
            {
                Template = new Template
                {
                    Recipe = Recipe.ChromePdf,
                    Engine = Engine.None,
                    Content = templatka
                }
            });

            //
            return File(report.Content, "application/pdf", "file.pdf");
        }

        const string templatka = 
            "<style>"
            + ".title {"
            +   "font-size: 22px;"
            +  "font-weight: 100;"
            +   "padding: 10px 20px 0px 20px;" 
            + "text-align: center;"
            +  "}"
            +     ".title span"
            +"{"
            + "color: #007cae;"
            +  "}"
            + ".details {"
            +    "padding: 10px 20px 0px 20px;"
            +    "text-align: left !important;"
            + "}"
            + "table{"
            +   "width: 100%;"
            +   "table-layout: fixed;"
            +"}"
            + "th {"
            +    "background-color: rgb(95, 95, 95);"
            +"color: white;"
            +     "text-align: center;"
            +     "opacity: 0.7;"
            +  "}"
            + "th, td{"
            +     "border-bottom: 1px solid #ddd;"
            + "}"
            +  "tr:hover {"
            +      "background-color: #f5f5f5;"
            +  "}"
            +   "tr:nth-child(even)"
            +"{"
            + "background - color: #f2f2f2;"
            +       "}"
            + "</style>"
            + "<div class='wrapper'>"
            +   "<div class='header' style='text-align: center'>"
            +       "<p class='title'>Invoice # </p>"
            + "</div>"
            + "<div>"
            +   "<div class='details'>"
            +       "<p>Created by: <span style='font-weight: 600;'> Name </span></p>"
            +       "<p>Date: <span style='font-weight: 600;'> Date </span></p>"
            +       "<p>Description: <span>Text</span></p>"
            +       "<hr style='opacity: 0.7;'/>"
            +   "</div>"
            +   "<div class='details'>"

            +   "</div>"
            + "</div>";
    
    }
}