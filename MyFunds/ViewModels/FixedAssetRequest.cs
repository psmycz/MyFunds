using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.ViewModels
{
    public class FixedAssetRequest
    {
        [Required]
        public bool InUse { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value for {0} must be bigger than {1}")]
        public double Price { get; set; }

        [Required]
        [Range(typeof(DateTime), "1/1/1950", "1/1/2100", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Range(typeof(DateTime), "1/1/1950", "1/1/2100", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime WarrantyEndDate { get; set; }

        /// <summary>
        /// If type != Rentable then cannot be assigned to user/InUse
        /// </summary>
        [Required]
        private string type;
        public string Type
        {
            get => type;
            set 
            {
                if (string.IsNullOrEmpty(value)) 
                    return;

                if (Enum.TryParse(value, out FixedAssetType _))
                {
                    type = value;
                }
            }
        }

        public int UserId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field RoomId is required and its value must be a positive integer")]
        public int RoomId { get; set; }
    }
}
