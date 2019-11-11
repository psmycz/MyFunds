using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class FixedAsset
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public bool InUse { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public DateTime WarrantyEndDate { get; set; }
        public FixedAssetType Type { get; set; }


        [Required]
        public int RoomId { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }

        public int? UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<FixedAssetArchive> FixedAssetArchives { get; set; }
    }

    public enum FixedAssetType 
    { 
        Undefined, 
        Static, 
        Rentable 
    }
}
