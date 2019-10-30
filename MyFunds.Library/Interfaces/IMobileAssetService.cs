using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IMobileAssetService
    {
        MobileAssetDTO GetMobileAsset(int mobileAssetId);
        List<MobileAssetDTO> GetAllMobileAssets();
        MobileAssetDTO Create(MobileAssetDTO mobileAssetDTO);
    }
}
