using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lebonanimal.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    [Required]
    [MaxLength(50)]
    [DisplayName("Titre du produit")]
    public string Title { get; set; }

    [Required] [DisplayName("Prix")]
    public float Price { get; set; }
    [Required] 
    public string ImgPath { get; set; }

    [Required]
    [DisplayName("Description")]
    [DataType(DataType.Text)]
    public string Description { get; set; }
    
    [DisplayName("Certificat en PDF pour les animaux supérieur à 500€")]
    public string? Certificat { get; set; }
    
    [DefaultValue(false)]
    public bool Enabled { get; set; }
    
    [DefaultValue(false)]
    public bool Deleted { get; set; }

    [InverseProperty(nameof(Order.ProductIdNavigation))]
    public virtual ICollection<Order> Orders { get; set; }

}