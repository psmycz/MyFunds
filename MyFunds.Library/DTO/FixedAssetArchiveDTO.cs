using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class FixedAssetArchiveDTO
    {
        public int Id { get; set; }


        public bool InUse { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public DateTime TimeStamp { get; set; }
        public FixedAssetType Type { get; set; }
        public string TypeName
        {
            get => Type.ToString();
        }


        public int? UserId { get; set; }
        public UserDTO User { get; set; }

        public int RoomId { get; set; }
        public RoomDTO Room { get; set; }

        public int FixedAssetId { get; set; }
        public FixedAssetDTO FixedAsset { get; set; }

    }
}
