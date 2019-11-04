using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IMobileAssetArchiveService : IBaseService<IMobileAssetArchiveService>
    {
        MobileAssetArchiveDTO AddArchive(int mobileAssetId);
    }
}
