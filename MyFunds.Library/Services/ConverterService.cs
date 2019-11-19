using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class ConverterService : BaseService<ConverterService>, IConverterService
    {
        public ConverterService(IMapper mapper)
            : base(mapper)
        { 
        }

        public MemoryStream ConvertJsonToExcel(string jsonString, string sheetName = "Sheet1")
        {
            var data = ConvertJsonToListOfObjects(jsonString);

            var memoryStream = new MemoryStream();

            using (SpreadsheetDocument spreedDoc = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
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
                    Name = sheetName
                };
                var workingSheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;


                var convertedData = ConvertJsonObjectsToDictionaryStructure(data);
                var listOfPropertyNames = convertedData.newData.SelectMany(r => r.value).Select(p => p.name).Distinct().ToList();
                AddObjectsPropertiesToGivenList(listOfPropertyNames, convertedData.objects);

                //header
                var headerRow = new Row();
                foreach (var prop in listOfPropertyNames)
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
                    foreach (var prop in listOfPropertyNames)
                    {
                        var propValue = FindPropertyInDictionaryList(prop, recordProperties);

                        row.AppendChild(
                            GetCell(propValue != null && propValue != "magicString" ? propValue : string.Empty)
                        );
                    }
                    sheetData.AppendChild(row);
                }
                wbPart.Workbook.Sheets.AppendChild(sheet);
                wbPart.Workbook.Save();
            }

            return memoryStream;
        }

        private List<JObject> ConvertJsonToListOfObjects(string jsonString)
        {
            var jsonData = new List<JObject>();

            var json = JsonConvert.DeserializeObject(jsonString, new JsonSerializerSettings()
            {
                Error = (sender, args) =>
                {
                    args.ErrorContext.Handled = true;
                    throw new ApiException("An error occured while deserializing data");
                }
            });
            if (json is JArray && (json as JArray).ToList().TrueForAll(item => item is JObject))
            {
                (json as JArray).ToList().ForEach(obj => jsonData.Add((JObject)obj));
            }
            else
            {
                throw new ApiException("An error occured while deserializing data - make sure the format is a correct json array of json objects only");
            }

            return jsonData;
        }

        private (List<(string name, List<(string name, object value)> value)> newData, List<(string name, List<string> properties)> objects, List<(string name, List<string> properties)> arrays) ConvertJsonObjectsToDictionaryStructure(List<JObject> jData)
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
        private void AddPropertyToObject(List<(string name, object value)> newData, List<JProperty> jProperties, List<(string name, List<string> properties)> objects, List<(string name, List<string> properties)> arrays, string prefix = "")
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
                }

                // JObject
                if ((prop.Value is JObject) && !(prop.Value is JArray))
                {
                    newData.Add((name: prop.Name, value: new List<(string name, object value)>()));

                    // adding to list of objects
                    var obj = (prop.Value as JObject);

                    int index = prop.Name.LastIndexOf('_');
                    string cleanPropertyName = (index < 0) ? prop.Name : prop.Name.Skip(index + 1).ToString();

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
                    string cleanPropertyName = (index < 0) ? prop.Name : prop.Name.Skip(index + 1).ToString();

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
        private List<string> AddObjectsPropertiesToGivenList(List<string> propertyNames, List<(string name, List<string> properties)> objectsProperties)
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
        private string FindPropertyInDictionaryList(string propName, List<(string name, object value)> properties)
        {
            foreach (var p in properties)
            {
                if (p.name == propName && p.value is string)
                    return p.value as string;
                if (p.name == propName && p.value is List<(string name, object value)>)
                    return "OBJECT";
                if (p.name == propName && p.value is List<(string name, List<(string name, object value)> value)>)
                    return "ARRAY";
                if (p.name != propName && p.value is List<(string name, object value)>)
                {
                    string value = FindPropertyInDictionaryList(propName, p.value as List<(string name, object value)>);
                    if (value != "magicString")
                        return value;
                }
                if (p.name != propName && (p.value is List<(string name, List<(string name, object value)> value)>))
                {
                    var objectList = new List<List<(string name, object value)>>();
                    (p.value as List<(string name, List<(string name, object value)> value)>).ForEach(obj => objectList.Add(obj.value));

                    foreach (var obj in objectList)
                    {
                        string value = FindPropertyInDictionaryList(propName, obj);
                        if (value != "magicString")
                            return value;
                    }
                }
            }

            return "magicString";
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
