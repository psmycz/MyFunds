using System;
using System.Collections.Generic;
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
        readonly IMapper mapper;

        public AdminController(IUserService userService, UserManager<User> userManager, IFixedAssetService fixedAssetService, IMobileAssetService mobileAssetService, IRoomService roomService, IBuildingService buildingService, IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.fixedAssetService = fixedAssetService;
            this.mobileAssetService = mobileAssetService;
            this.roomService = roomService;
            this.buildingService = buildingService;
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
            return Ok(fixedAssetService.GetFixedAssetWithArchives(fixedAssetId));
        }

        [HttpGet]
        [Route("GetMobileAssetWithArchives/{mobileAssetId}")]
        public IActionResult GetMobileAssetWithArchives(int mobileAssetId)
        {
            return Ok(mobileAssetService.GetMobileAssetWithArchives(mobileAssetId));
        }

        [HttpGet]
        [Route("GetRoomWithAssets/{roomId}")]
        public IActionResult GetRoomWithAssets(int roomId)
        {
            return Ok(roomService.GetRoomWithAssets(roomId));
        }

        [HttpGet]
        [Route("GetBuildingWithAssets/{buildingId}")]
        public IActionResult GetBuildingWithAssets(int buildingId)
        {
            return Ok(buildingService.GetBuildingWithAssets(buildingId));
        }

        //TODO: fix placing properties in correct order for nested objects
        [HttpPost]
        [Route("ExportExcel")]
        public async Task<IActionResult> ExportExcel([FromQuery] string fileName = "file")
        {
            var streamReader = new StreamReader(HttpContext.Request.Body);
            //streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            var requestBody = streamReader.ReadToEnd();

            List<JObject> data = new List<JObject>();

            var json = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() 
            {
                Error = (sender, args) => 
                {
                    args.ErrorContext.Handled = true;
                    throw new ApiException("An error occured while deserializing data");
                }
            });
            if (json is JArray && (json as JArray).ToList().TrueForAll(item => item is JObject))
            {
                (json as JArray).ToList().ForEach(obj => data.Add((JObject)obj));  
            }
            else
            {
                throw new ApiException("An error occured while deserializing data - make sure the format is a correct json array of json objects only");
            }


            HttpContext.Response.Headers["Content-Disposition"] =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName + ".xlsx"
                }.ToString();
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";



            var ms = new MemoryStream();

            using (SpreadsheetDocument spreedDoc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //openxml stuff
                var wbPart = spreedDoc.AddWorkbookPart();
                wbPart.Workbook = new Workbook();
                var worksheetPart = wbPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);
                wbPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet()
                {
                    Id = wbPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                };
                var workingSheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;

                //get model properties
                var props = new List<KeyValuePair<string, object>>(); 
                props.AddRange(data.First().Properties().Select(prop => new KeyValuePair<string, object>(prop.Name, !(prop.Value is JObject)? prop.Value : AddProps(prop) )).ToList());  //new List<JProperty>();//data.Properties());

                string AddProps(JProperty prop)
                {
                    var properties = (prop.Value as JObject).Properties().Select(p => new KeyValuePair<string, object>(p.Name, !(p.Value is JObject) ? p.Value : AddProps(p))).ToList();
                    props.AddRange(properties);
                    return string.Empty;
                }

                //header
                var headerRow = new Row();
                foreach (var prop in props)
                {
                    headerRow.AppendChild(
                        GetCell(prop.Key)
                    );
                }
                sheetData.AppendChild(headerRow);

                //body
                foreach (JObject record in data)
                {
                    var recordProperties = record.Properties().ToList();

                    var row = new Row();
                    foreach (var prop in props)
                    {
                        var propValue = Find(prop.Key, recordProperties);

                        string Find(string propName, List<JProperty> properties)
                        {
                            foreach (var p in properties)
                            {
                                if (p.Name == propName && !(p.Value is JObject))
                                    return p.Value.ToString();
                                if (p.Name == propName && (p.Value is JObject))
                                    return string.Empty;
                                if (p.Name != propName && (p.Value is JObject))
                                {
                                    string value = Find(propName, (p.Value as JObject).Properties().ToList());
                                    if (value != "magicString")
                                        return value;
                                }
                            }

                            return "magicString";
                        }

                        row.AppendChild(
                            GetCell(!(propValue == null || propValue == "magicString") ? propValue : string.Empty)
                        );
                    }
                    sheetData.AppendChild(row);
                }
                wbPart.Workbook.Sheets.AppendChild(sheet);
                wbPart.Workbook.Save();
            }


            HttpContext.Response.ContentLength = ms.Length;
            await HttpContext.Response.Body.WriteAsync(ms.ToArray());

            return Ok();
        }

        private Cell GetCell(string text)
        {
            var cell = new Cell()
            {
                DataType = CellValues.InlineString
            };
            var inlineString = new InlineString();
            inlineString.AppendChild(new Text(text));

            cell.AppendChild(inlineString);
            return cell;
        }

    }
}