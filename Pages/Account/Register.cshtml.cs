using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Helpers;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserService _users;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public RegisterModel(UserService users)
    {
        _users = users;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_users.EmailExists(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Cet email est déjà utilisé");
            return Page();
        }

        var user = new User
        {
            Nom = Input.Nom,
            Email = Input.Email,
            PasswordHash = PasswordHelper.Hash(Input.Password),
            Role = "Viewer"
        };

        _users.Create(user);

        TempData["Message"] = "Compte créé. Tu peux te connecter";
        return RedirectToPage("Login");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        public string Nom { get; set; } = "";

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [StringLength(200)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [MinLength(8, ErrorMessage = "Au moins 8 caractères")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; } = "";
    }
}
