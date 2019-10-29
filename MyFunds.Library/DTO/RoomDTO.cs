using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }


        /// <summary>
        /// Area in square meters
        /// </summary>
        public double Area { get; set; }
        public int Floor { get; set; }
        public RoomType Type { get; set; }
        public string TypeName
        {
            get => Type.ToString();
        }


        public int BuildingId { get; set; }
        public Building Building { get; set; }
    }
}
