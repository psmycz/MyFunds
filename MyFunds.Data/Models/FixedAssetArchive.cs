using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class FixedAssetArchive
    {
        [Key]
        public int Id { get; set; }


        public bool InUse { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public FixedAssetType Type { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }


        [Required]
        public int RoomId { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [Required]
        public int FixedAssetId { get; set; }
        public virtual FixedAsset FixedAsset { get; set; }
    }
}
