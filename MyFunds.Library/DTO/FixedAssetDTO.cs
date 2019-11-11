using AutoMapper.Configuration.Annotations;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class FixedAssetDTO
    {
        public int Id { get; set; }


        public bool InUse { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public FixedAssetType Type { get; set; }
        public string TypeName 
        {
            get => Type.ToString(); 
        }


        public int? UserId { get; set; }
        public UserDTO User { get; set; }

        public int RoomId { get; set; }
        public RoomDTO Room{ get; set; }

        [Ignore]
        public FixedAssetArchiveDTO PreviousFixedAsset { get; set; }
        [Ignore]
        public List<FixedAssetArchiveDTO> FixedAssetArchives { get; set; }

    }
}
