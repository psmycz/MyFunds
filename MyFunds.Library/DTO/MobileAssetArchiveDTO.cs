using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class MobileAssetArchiveDTO
    {
        public int Id { get; set; }


        public bool InUse { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public DateTime TimeStamp { get; set; }


        public int UserId { get; set; }
        public UserDTO User { get; set; }


        public int MobileAssetId { get; set; }
        public MobileAssetDTO MobileAsset { get; set; }
    }
}
