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



        [HttpGet]
        [Route("GetUser/{userId}")]
        public IActionResult GetUser(int userId)
        {
            return Ok(userService.GetUser(userId));
        }
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            return Ok(userService.GetAllUsers());
        }
        


        [HttpGet]
        [Route("GetFixedAsset/{fixedAssetId}")]
        public IActionResult GetFixedAsset(int fixedAssetId)
        {
            return Ok(fixedAssetService.GetFixedAsset(fixedAssetId));
        }
        [HttpGet]
        [Route("GetAllFixedAssets")]
        public IActionResult GetAllFixedAssets()
        {
            return Ok(fixedAssetService.GetAllFixedAssets());
        }


        [HttpGet]
        [Route("GetMobileAsset/{mobileAssetId}")]
        public IActionResult GetMobileAsset(int mobileAssetId)
        {
            return Ok(mobileAssetService.GetMobileAsset(mobileAssetId));
        }
        [HttpGet]
        [Route("GetAllMobileAssets")]
        public IActionResult GetAllMobileAssets()
        {
            return Ok(mobileAssetService.GetAllMobileAssets());
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
        [HttpGet]
        [Route("GetMeWithAssets")]
        public IActionResult GetMeWithAssets()
        {
            var id = userManager.GetUserId(User);

            int userId;
            if (!int.TryParse(id, out userId))
                throw new ApiException("Incorrect user Id");

            var user = userService.GetUserWithAssets(userId);

            return Ok(user);
        }


        [HttpGet]
        [Route("GetRoom/{roomId}")]
        public IActionResult GetRoom(int roomId)
        {
            return Ok(roomService.GetRoom(roomId));
        }
        [HttpGet]
        [Route("GetAllRooms")]
        public IActionResult GetAllRooms()
        {
            return Ok(roomService.GetAllRooms());
        }


        [HttpGet]
        [Route("GetBuilding/{buildingId}")]
        public IActionResult GetBuilding(int buildingId)
        {
            return Ok(buildingService.GetBuilding(buildingId));
        }
        [HttpGet]
        [Route("GetAllBuildings")]
        public IActionResult GetAllBuildings()
        {
            return Ok(buildingService.GetAllBuildings());
        }


        [HttpPost]
        [Route("AddFixedAsset")]
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
            if (fixedAsset.InUse && (fixedAsset.Type != FixedAssetType.Rentable.ToString() || fixedAsset.UserId == 0))
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
                return new ValidationProblemDetailsResult();
            }
            if (!fixedAsset.InUse && fixedAsset.UserId != 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.UserId != 0 && !userService.UserExist(fixedAsset.UserId))
            {
                ModelState.AddModelError(nameof(fixedAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.RoomId != 0 && !roomService.RoomExist(fixedAsset.RoomId))
            {
                ModelState.AddModelError(nameof(fixedAsset.RoomId), $"Room with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }


            var fixedAssetDTO = mapper.Map<FixedAssetDTO>(fixedAsset);
            var newAsset = fixedAssetService.Create(fixedAssetDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetFixedAsset", "User", values: new { fixedAssetId = newAsset.Id });
            return Created(link, newAsset);
        }
        [HttpPost]
        [Route("UpdateFixedAsset")]
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
            if (fixedAsset.InUse && (fixedAsset.Type != FixedAssetType.Rentable.ToString() || fixedAsset.UserId == 0))
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
                return new ValidationProblemDetailsResult();
            }
            if (!fixedAsset.InUse && fixedAsset.UserId != 0)
            {
                ModelState.AddModelError(nameof(fixedAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (fixedAsset.UserId != 0 && !userService.UserExist(fixedAsset.UserId))
            {
                ModelState.AddModelError(nameof(fixedAsset.UserId), $"User with provided Id does not exist");
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


        [HttpPost]
        [Route("AddMobileAsset")]
        public IActionResult AddMobileAsset(MobileAssetRequest mobileAsset)
        {
            if (mobileAsset.InUse && mobileAsset.UserId == 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"Must provide UserId when InUse");
                return new ValidationProblemDetailsResult();
            }
            if (!mobileAsset.InUse && mobileAsset.UserId != 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (mobileAsset.UserId != 0 && !userService.UserExist(mobileAsset.UserId))
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }
            if (DateTime.Compare(mobileAsset.PurchaseDate, mobileAsset.WarrantyEndDate) >= 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"Purchase Date must be earlier than Warranty End Date");
                return new ValidationProblemDetailsResult();
            }


            var mobileAssetDTO = mapper.Map<MobileAssetDTO>(mobileAsset);
            var newAsset = mobileAssetService.Create(mobileAssetDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetMobileAsset", "User", values: new { mobileAssetId = newAsset.Id });
            return Created(link, newAsset);
        }
        [HttpPost]
        [Route("UpdateMobileAsset")]
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
            if (mobileAsset.InUse && mobileAsset.UserId == 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.InUse), $"UserId must be provided when InUse");
                return new ValidationProblemDetailsResult();
            }
            if (!mobileAsset.InUse && mobileAsset.UserId != 0)
            {
                ModelState.AddModelError(nameof(mobileAsset.InUse), $"While not in use, cannot provide userId");
                return new ValidationProblemDetailsResult();
            }
            if (mobileAsset.UserId != 0 && !userService.UserExist(mobileAsset.UserId))
            {
                ModelState.AddModelError(nameof(mobileAsset.UserId), $"User with provided Id does not exist");
                return new ValidationProblemDetailsResult();
            }

            var mobileAssetDTO = mapper.Map<MobileAssetDTO>(mobileAsset);
            var newAsset = mobileAssetService.Update(mobileAssetDTO);

            return Ok(newAsset);
        }


        // add room
        [HttpPost]
        [Route("AddRoom")]
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


            var link = linkGenerator.GetUriByAction(HttpContext, "GetRoom", "User", values: new { roomId = newRoom.Id });
            return Created(link, newRoom);
        }
        [HttpPost]
        [Route("UpdateRoom")]
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


        // add building
        [HttpPost]
        [Route("AddBuilding")]
        public IActionResult AddBuilding(BuildingRequest building)
        {

            var buildingDTO = mapper.Map<BuildingDTO>(building);
            var newBuilding = buildingService.Create(buildingDTO);


            var link = linkGenerator.GetUriByAction(HttpContext, "GetBuilding", "User", values: new { buildingId = newBuilding.Id });
            return Created(link, newBuilding);
        }
        [HttpPost]
        [Route("UpdateBuilding")]
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
    }
}