using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class MobileAssetArchive
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }
        public bool InUse { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }


        public int? UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int MobileAssetId { get; set; }
        public virtual MobileAsset MobileAsset { get; set; }
    }
}
