using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace lebonanimal.Models;

public class UserLogin
{

    [Required(ErrorMessage = "le champ est vide")]
    [EmailAddress]
    [DisplayName("Email")]
    [MaxLength(320,ErrorMessage = "La taille max est 320")]
    public string Email { get; set; }

    [Required(ErrorMessage = "le champ est vide")]
    [MinLength(10, ErrorMessage = "Le mot de passe doit faire au moins 10 caractères")] 
    [DisplayName("Mot de passe")]
    public string Password { get; set; }

}