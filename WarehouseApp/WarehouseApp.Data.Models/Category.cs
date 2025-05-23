﻿using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Data.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
             = new HashSet<Product>();
    }
}
