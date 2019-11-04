using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// Area in square meters
        /// </summary>
        public double Area { get; set; }
        public int Floor { get; set; }
        public RoomType Type { get; set; }


        [Required]
        public int BuildingId { get; set; }
        public virtual Building Building { get; set; }

        public virtual ICollection<FixedAsset> FixedAssets { get; set; }

        [JsonIgnore]
        public virtual ICollection<FixedAssetArchive> FixedAssetArchives { get; set; }

    }

    public enum RoomType 
    { 
        Undefined, 
        Workplace, 
        Sanitary, 
        Warehouse, 
        Other 
    }
}
