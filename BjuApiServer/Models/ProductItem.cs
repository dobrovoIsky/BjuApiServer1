using System.ComponentModel.DataAnnotations;

namespace BjuApiServer.Models
{
    public class ProductItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public double CaloriesPer100g { get; set; }
        public double ProteinPer100g { get; set; }
        public double FatPer100g { get; set; }
        public double CarbsPer100g { get; set; }
    }
}
