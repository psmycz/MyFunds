using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class User : IdentityUser<int>
    {
        [Key]
        override public int Id { get; set; }


        public virtual ICollection<FixedAsset> FixedAssets { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<FixedAssetArchive> FixedAssetArchives { get; set; }

        public virtual ICollection<MobileAsset> MobileAssets { get; set; }

        [JsonIgnore]
        public virtual ICollection<MobileAssetArchive> MobileAssetArchives { get; set; }
    }
}
