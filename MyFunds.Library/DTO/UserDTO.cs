using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        public List<FixedAssetDTO> FixedAssets { get; set; }
        public List<MobileAssetDTO> MobileAssets { get; set; }
    }
}
