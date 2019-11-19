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
        public IActionResult GetAllUsersWithAssets()
        {
            return Ok(userService.GetAllUsersWithAssets());
        }

        /// <summary>
        /// Get all users with their assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetFixedAssetWithArchives/{fixedAssetId}")]
        [ProducesResponseType(typeof(FixedAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetFixedAssetWithArchives(int fixedAssetId)
        {
            return Ok(fixedAssetService.GetFixedAssetWithArchives(fixedAssetId));
        }

        /// <summary>
        /// Get all users with their assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetMobileAssetWithArchives/{mobileAssetId}")]
        [ProducesResponseType(typeof(MobileAssetDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetMobileAssetWithArchives(int mobileAssetId)
        {
            return Ok(mobileAssetService.GetMobileAssetWithArchives(mobileAssetId));
        }

        /// <summary>
        /// Get all users with their assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetRoomWithAssets/{roomId}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetRoomWithAssets(int roomId)
        {
            return Ok(roomService.GetRoomWithAssets(roomId));
        }

        /// <summary>
        /// Get all users with their assets
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpGet]
        [Route("GetBuildingWithAssets/{buildingId}")]
        [ProducesResponseType(typeof(BuildingDTO), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetBuildingWithAssets(int buildingId)
        {
            return Ok(buildingService.GetBuildingWithAssets(buildingId));
        }

        //TODO: now cannot contain duplicates, need prefix or sth
        //TODO: make new sheet for every array in json stringc
        /// <summary>
        /// Export excel from given json
        /// </summary>
        /// <response code="400">Unable to finish request due to an error</response>
        [HttpPost]
        [Route("ExportExcel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
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

                var propy = new List<KeyValuePair<string, object>>();

                var listOfObjects = new List<KeyValuePair<string, List<string>>>();
                var listOfArrays = new List<KeyValuePair<string, List<string>>>();

                //data.ForEach(obj => PropyAddWithPrefix(obj.Properties().ToList()));
                
                void PropyAdd(List<JProperty> jProperties)
                {
                    foreach (var prop in jProperties)
                    {
                        if(!prop.HasValues)
                        {
                            throw new ApiException("Json contains na object without property - JValue - provided json must contain only JObjects");
                        }

                        // Good prop
                        if (!(prop.Value is JObject) && !(prop.Value is JArray))
                        {
                            propy.Add(new KeyValuePair<string, object>(prop.Name, prop.Value));
                        }

                        // JObject
                        if ((prop.Value is JObject) && !(prop.Value is JArray))
                        {
                            propy.Add(new KeyValuePair<string, object>(prop.Name, "object"));
                            
                            var obj = (prop.Value as JObject);
                            var objProperties = obj.Properties().ToList().Select(p => p.Name).ToList();

                            if (!listOfObjects.Any(o => o.Key == prop.Name))
                            {
                                listOfObjects.Add(new KeyValuePair<string, List<string>>(prop.Name, objProperties));
                            }

                            var properties = listOfObjects.First(k => k.Key == prop.Name).Value;
                            foreach (var p in objProperties)
                            {
                                if (!properties.Any(propoperty => propoperty == p))
                                {
                                    properties.Add(p);
                                }
                            }

                            PropyAdd(obj.Properties().ToList());
                        }

                        // JArray
                        if (!(prop.Value is JObject) && (prop.Value is JArray))
                        {
                            propy.Add(new KeyValuePair<string, object>(prop.Name, "array"));
                            
                            List<JObject> jObjectList = new List<JObject>();

                            var jArray = (prop.Value as JArray).ToList();
                            if (jArray.Count != 0 && jArray.TrueForAll(o => o is JObject))
                            {
                                jArray.ForEach(obj => jObjectList.Add((JObject)obj));
                            }
                            else
                            {
                                throw new ApiException("Provided json must contain only JObjects");
                            }

                            var arrayProperties = jObjectList.SelectMany(o => o.Properties()).Select(p => p.Name).Distinct().ToList();
                            
                            if (!listOfArrays.Any(o => o.Key == prop.Name))
                            {
                                listOfArrays.Add(new KeyValuePair<string, List<string>>(prop.Name, arrayProperties));
                            }

                            var properties = listOfArrays.First(k => k.Key == prop.Name).Value;
                            foreach (var p in arrayProperties)
                            {
                                if (!properties.Any(propoperty => propoperty == p))
                                {
                                    properties.Add(p);
                                }
                            }

                            jObjectList.ForEach(obj => PropyAdd(obj.Properties().ToList()));
                        }

                    }
                }







                (List<(string name, List<(string name, object value)> value)> newData, List<(string name, List<string> properties)> objects, List<(string name, List<string> properties)> arrays) SomeFunction(List<JObject> jData)
                {
                    var newData = new List<(string name, List<(string name, object value)> value)>();

                    var objects = new List<(string name, List<string> properties)>();
                    var arrays = new List<(string name, List<string> properties)>();


                    for (int i = 0; i < jData.Count; i++)
                    {
                        var listOfProps = jData[i].Properties().ToList();
                        newData.Add((name: string.Empty, value: new List<(string name, object value)>()));

                        AddPropertyToObject(newData[i].value, listOfProps, objects, arrays);
                    }

                    return (newData, objects, arrays);
                }


                void AddPropertyToObject(List<(string name, object value)> newData, List<JProperty> jProperties, List<(string name, List<string> properties)> objects, List<(string name, List<string> properties)> arrays, string prefix = "")
                {
                    foreach (var prop in jProperties)
                    {
                        if (!prop.HasValues)
                        {
                            throw new ApiException("Json contains na object without property - JValue - provided json must contain only JObjects");
                        }

                        // Good prop
                        if (!(prop.Value is JObject) && !(prop.Value is JArray))
                        {
                            newData.Add((name: prefix + prop.Name, value: prop.Value.ToString()));

                            //propy.Add(new KeyValuePair<string, object>(prefix + prop.Name, prop.Value));
                            
                        }

                        // JObject
                        if ((prop.Value is JObject) && !(prop.Value is JArray))
                        {
                            newData.Add((name: prop.Name, value: new List<(string name, object value)>()));

                            //propy.Add(new KeyValuePair<string, object>(prefix + prop.Name, "object"));
                            
                            // adding to list of objects
                            var obj = (prop.Value as JObject);

                            int index = prop.Name.LastIndexOf('_');
                            string cleanPropertyName;
                            if (index < 0)
                                cleanPropertyName = prop.Name;
                            else
                                cleanPropertyName = prop.Name.Skip(index + 1).ToString();

                            var objProperties = obj.Properties().ToList().Select(p => !(p.Value is JObject || p.Value is JArray) ? cleanPropertyName + "_" + p.Name : p.Name).ToList();

                            if (!objects.Any(o => o.name == prop.Name))
                            {
                                objects.Add((prop.Name, objProperties));
                            }
                            else
                            {
                                var properties = objects.First(o => o.name == prop.Name).properties;
                                foreach (var p in objProperties)
                                {
                                    if (!properties.Any(propoperty => propoperty == p))
                                    {
                                        properties.Add(p);
                                    }
                                }
                            }

                            AddPropertyToObject(newData.First(v => v.name == prop.Name).value as List<(string name, object value)>, obj.Properties().ToList(), objects, arrays, prop.Name + "_");
                        }

                        // JArray
                        if (!(prop.Value is JObject) && (prop.Value is JArray))
                        {
                            newData.Add((name: prop.Name, value: new List<(string name, List<(string name, object value)> value)>()));

                            //propy.Add(new KeyValuePair<string, object>(prefix + prop.Name, "array"));

                            List<JObject> jObjectList = new List<JObject>();

                            var jArray = (prop.Value as JArray).ToList();
                            if (jArray.Count != 0 && jArray.TrueForAll(o => o is JObject))
                            {
                                jArray.ForEach(obj => jObjectList.Add((JObject)obj));
                            }
                            else
                            {
                                throw new ApiException("Provided json must contain only JObjects");
                            }

                            int index = prop.Name.LastIndexOf('_');
                            string cleanPropertyName;
                            if (index < 0)
                                cleanPropertyName = prop.Name;
                            else
                                cleanPropertyName = prop.Name.Skip(index + 1).ToString();

                            var arrayProperties = jObjectList.SelectMany(o => o.Properties()).Select(p => !(p.Value is JObject || p.Value is JArray) ? cleanPropertyName + "_" + p.Name : p.Name).Distinct().ToList();

                            if (!arrays.Any(o => o.name == prop.Name))
                            {
                                arrays.Add((prop.Name, arrayProperties));
                            }
                            else
                            {
                                var properties = arrays.First(o => o.name == prop.Name).properties;
                                foreach (var p in arrayProperties)
                                {
                                    if (!properties.Any(propoperty => propoperty == p))
                                    {
                                        properties.Add(p);
                                    }
                                }
                            }
                            //jObjectList.ForEach(obj => PropyAddWithPrefix(obj.Properties().ToList(), prop.Name + "_"));
                            
                            for (int i = 0; i < jObjectList.Count; i++)
                            {
                                var listOfProps = jObjectList[i].Properties().ToList();
                                var newDataValue = newData.First(o => o.name == prop.Name).value as List<(string name, List<(string name, object value)> value)>;

                                newDataValue.Add((name: string.Empty, value: new List<(string name, object value)>()));

                                AddPropertyToObject(newDataValue[i].value, listOfProps, objects, arrays, prop.Name + "_");
                            }
                        }

                    }
                }

                var convertedData = SomeFunction(data);

                var listOfPropertyNames = convertedData.newData.SelectMany(r => r.value).Select(p => p.name).Distinct().ToList();
                
                List<string> AddObjectsPropertiesToGivenList(List<string> propertyNames, List<(string name, List<string> properties)> objectsProperties)
                {
                    int existingPropertiesCount;
                    do
                    {
                        existingPropertiesCount = propertyNames.Count;

                        foreach (var p in objectsProperties)
                        {
                            int index = propertyNames.FindIndex(n => n == p.name);

                            if (index == -1) continue;

                            var alreadyExistingProperties = propertyNames.Skip(index + 1).Take(p.properties.Count).ToList();

                            if (alreadyExistingProperties?.Count != 0 && alreadyExistingProperties.TrueForAll(propName => p.properties.Contains(propName) || p.properties.TrueForAll(prop => propertyNames.Contains(prop)))) continue;

                            propertyNames.InsertRange(index + 1, p.properties);
                        }
                    }
                    while (existingPropertiesCount != propertyNames.Count);

                    return propertyNames;
                }

                AddObjectsPropertiesToGivenList(listOfPropertyNames, convertedData.objects);






                var list = propy.Select(p => p.Key).Distinct().ToList();
                //var listOfObjectPropertiesWithoutArrays = listOfObjects.SelectMany(o => o.Value).Select(name => name).Where(name => !listOfArrays.Any(k => k.Key == name)).ToList();
                
                var arrayProp = data.SelectMany(o => o.Properties()).Select(p => p.Name).Distinct().ToList();


                int prevLength = arrayProp.Count;
                AddObjectProps();

                void AddObjectProps()
                {
                    foreach (var p in listOfObjects)
                    {
                        int index = arrayProp.FindIndex(n => n == p.Key);

                        if (index == -1) continue;

                        var objectProps = arrayProp.Skip(index + 1).Take(p.Value.Count).ToList();

                        if (objectProps?.Count != 0 && objectProps.TrueForAll(o => p.Value.Contains(o))) continue;

                        arrayProp.InsertRange(index + 1, p.Value);
                    }
                }

                while (prevLength != arrayProp.Count)
                {
                    prevLength = arrayProp.Count;
                    AddObjectProps();
                }

                /*
                                int prevLength = arrayProp.Count;
                                AddArrayProps();

                                void AddArrayProps()
                                {
                                    foreach (var p in listOfArrays)
                                    {
                                        int index = arrayProp.FindIndex(n => n == p.Key);

                                        if (index == -1) continue;

                                        arrayProp.InsertRange(index + 1, p.Value);
                                    }
                                }

                                while (prevLength != arrayProp.Count)
                                {
                                    prevLength = arrayProp.Count;
                                    AddArrayProps();
                                }
                */

                /*//header
                var headerRow = new Row();
                foreach (var prop in arrayProp)//list)//props)
                {
                    headerRow.AppendChild(
                        GetCell(prop)
                    );
                }
                sheetData.AppendChild(headerRow);
                
                //body
                foreach (JObject record in data)
                {
                    var recordProperties = record.Properties().ToList();

                    var row = new Row();
                    foreach (var prop in arrayProp)//list)//props)
                    {
                        var propValue = Find(prop, recordProperties);

                        string Find(string propName, List<JProperty> properties)
                        {
                            foreach (var p in properties)
                            {
                                if (p.Name == propName && !(p.Value is JObject) && !(p.Value is JArray))
                                    return p.Value.ToString();
                                if (p.Name == propName && (p.Value is JObject) && !(p.Value is JArray))
                                    return "OBJECT";//string.Empty;
                                if (p.Name == propName && (p.Value is JArray))
                                    return "ARRAY";//string.Empty;
                                if (p.Name != propName && (p.Value is JObject))
                                {
                                    string value = Find(propName, (p.Value as JObject).Properties().ToList());
                                    if (value != "magicString")
                                        return value;
                                }
                                if (p.Name != propName && (p.Value is JArray))
                                {
                                    List<JObject> jObjectList = new List<JObject>();
                                    (p.Value as JArray).ToList().ForEach(obj => jObjectList.Add((JObject)obj));

                                    foreach (JObject obj in jObjectList)
                                    {
                                        string value = Find(propName, obj.Properties().ToList());
                                        if (value != "magicString")
                                            return value;
                                    }
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
                wbPart.Workbook.Save();*/

                //header
                var headerRow = new Row();
                foreach (var prop in listOfPropertyNames)//list)//props)
                {
                    headerRow.AppendChild(
                        GetCell(prop)
                    );
                }
                sheetData.AppendChild(headerRow);

                //body
                foreach (var record in convertedData.newData)
                {
                    var recordProperties = record.value;

                    var row = new Row();
                    foreach (var prop in listOfPropertyNames)//list)//props)
                    {
                        var propValue = Find(prop, recordProperties);

                        string Find(string propName, List<(string name, object value)> properties)
                        {
                            foreach (var p in properties)
                            {
                                if (p.name == propName && p.value is string)
                                    return p.value as string;
                                if (p.name == propName && p.value is List<(string name, object value)>)
                                    return "OBJECT";//string.Empty;
                                if (p.name == propName && p.value is List<(string name, List<(string name, object value)> value)>)
                                    return "ARRAY";//string.Empty;
                                if (p.name != propName && p.value is List<(string name, object value)>)
                                {
                                    string value = Find(propName, p.value as List<(string name, object value)>);
                                    if (value != "magicString")
                                        return value;
                                }
                                if (p.name != propName && (p.value is List<(string name, List<(string name, object value)> value)>))
                                {
                                    var objectList = new List<List<(string name, object value)>>();
                                    (p.value as List<(string name, List<(string name, object value)> value)>).ForEach(obj => objectList.Add(obj.value));

                                    foreach (var obj in objectList)
                                    {
                                        string value = Find(propName, obj);
                                        if (value != "magicString")
                                            return value;
                                    }
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