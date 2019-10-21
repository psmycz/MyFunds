using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }


        public virtual Address Address { get; set; }

        [JsonIgnore]
        public virtual ICollection<Room> Rooms { get; set; }

    }
}
