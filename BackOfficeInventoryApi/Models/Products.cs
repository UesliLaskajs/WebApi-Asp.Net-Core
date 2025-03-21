﻿using System.ComponentModel.DataAnnotations;

namespace BackOfficeInventoryApi.Models
{
    public class Products
    {
        
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
    
        public string? Description { get; set; }

        public int? Quantity { get; set; } = 0;
    }
}
