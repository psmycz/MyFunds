using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IFixedAssetService : IBaseService<IFixedAssetService>
    {
        bool FixedAssetExist(int fixedAssetId);
        FixedAssetDTO GetFixedAsset(int fixedAssetId);
        List<FixedAssetDTO> GetAllFixedAssets();
        FixedAssetDTO Create(FixedAssetDTO fixedAssetDTO);  
        FixedAssetDTO Update(FixedAssetDTO fixedAssetDTO);  
    }
}
