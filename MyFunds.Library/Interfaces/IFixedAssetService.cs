using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IFixedAssetService
    {
        FixedAssetDTO GetFixedAsset(int fixedAssetId);
        List<FixedAssetDTO> GetAllFixedAssets();
        FixedAssetDTO Create(FixedAssetDTO fixedAssetDTO);  
    }
}
