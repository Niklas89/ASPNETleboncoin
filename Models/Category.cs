using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace lebonanimal.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [DisplayName("Nom de la catégorie")]
    public string Name { get; set; }
}