

using System.ComponentModel.DataAnnotations;

namespace UniversRoom.Services.Identity.API.Models.AccountViewModels;

public class RegisterViewModel
{
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    public string Username { get; set; }

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }
    
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Required]
    public string ConfirmPassword { get; set; }
    
    public string? ReturnUrl { get; set; }
}