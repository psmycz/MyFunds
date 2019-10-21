using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Postcode { get; set; }


        [Required]
        public int BuildingId { get; set; }
        [JsonIgnore]
        public virtual Building Building { get; set; }
    }
}
