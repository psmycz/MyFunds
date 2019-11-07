using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.ViewModels
{
    public class RoomRequest
    {
        public int Id { get; set; }


        /// <summary>
        /// Area in square meters
        /// </summary>
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value for {0} must be bigger than {1}")]
        public double Area { get; set; }
        [Required]
        public int Floor { get; set; }
        [Required]
        private string type;
        public string Type
        {
            get => type;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                if (Enum.TryParse(value, out RoomType _))
                {
                    type = value;
                }
            }
        }


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field BuildingId is required and its value must be a positive integer")]
        public int BuildingId { get; set; }
    }
}
