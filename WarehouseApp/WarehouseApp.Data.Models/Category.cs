using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Data.Models
{
    [Comment("Categories in the system")]
    public class Category
    {
        [Comment("Category identifier")]
        public Guid Id { get; set; }

        [Comment("Name of the category ")]
        public required string Name { get; set; }

        [Comment("Description of the category , optional")]
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
             = new HashSet<Product>();
    }
}
