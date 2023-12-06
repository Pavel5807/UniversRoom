using System.ComponentModel.DataAnnotations;
using UniversRoom.Services.Identity.API.Controllers;

namespace UniversRoom.Services.Identity.API.Models.AccountViewModels;

public class LoginViewModel
{
    [Required]
    public  string Username { get; set; }

    [DataType(DataType.Password)]
    [Required]
    public  string Password { get; set; }
    
    public string? ReturnUrl { get; set; }
}