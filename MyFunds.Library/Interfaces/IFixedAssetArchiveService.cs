using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IFixedAssetArchiveService : IBaseService<IFixedAssetArchiveService>
    {
        FixedAssetArchiveDTO AddArchive(int fixedAssetId);
    }
}
