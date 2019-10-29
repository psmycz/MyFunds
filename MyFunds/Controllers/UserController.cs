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
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;
        readonly UserManager<User> userManager;
        readonly IFixedAssetService fixedAssetService;
        readonly IMobileAssetService mobileAssetService;
        readonly IRoomService roomService;

        public UserController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IRoomService roomService)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.roomService = roomService;
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



            // FixedAssetRequest to FixedAssetDTO convert
            var fixedAssetDTO = new FixedAssetDTO()
            {
                Name = fixedAsset.Name,
                InUse = fixedAsset.InUse,
                Price = fixedAsset.Price,
                PurchaseDate = fixedAsset.PurchaseDate.ToUniversalTime(),
                WarrantyEndDate = fixedAsset.WarrantyEndDate.ToUniversalTime(),
                Type = Enum.Parse<FixedAssetType>(fixedAsset.Type),
                RoomId = fixedAsset.RoomId,
                UserId = fixedAsset.UserId
            };

            var newAsset = fixedAssetService.Create(fixedAssetDTO);

            //return Created($"/FixedAsset/{newAsset.Id}", newAsset);
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

            // FixedAssetRequest to FixedAssetDTO convert
            var mobileAssetDTO = new MobileAssetDTO()
            {
                Name = mobileAsset.Name,
                InUse = mobileAsset.InUse,
                Price = mobileAsset.Price,
                PurchaseDate = mobileAsset.PurchaseDate.ToUniversalTime(),
                WarrantyEndDate = mobileAsset.WarrantyEndDate.ToUniversalTime(),
                UserId = mobileAsset.UserId
            };

            var newAsset = mobileAssetService.Create(mobileAssetDTO);

            //return Created($"/MobileAsset/{newAsset.Id}", newAsset);

            return Ok(newAsset);
        }
    }
}