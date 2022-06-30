using System.ComponentModel.DataAnnotations.Schema;

namespace Concepts.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        [ForeignKey("User")]
        public int CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
    }
}