using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IConverterService : IBaseService<IConverterService>
    {
        MemoryStream ConvertJsonToExcel(string jsonString, string sheetName = "Sheet1");
    }
}
