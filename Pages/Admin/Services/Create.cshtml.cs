using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Services;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ServiceService _services;

    [BindProperty]
    public Service Service { get; set; } = new();

    public List<SelectListItem> StatutOptions { get; } = new()
    {
        new("operational", "Opérationnel"),
        new("degraded", "Dégradé"),
        new("outage", "Panne")
    };

    public CreateModel(ServiceService services)
    {
        _services = services;
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

        _services.Create(Service);
        TempData["Message"] = $"Service '{Service.Nom}' créé";
        return RedirectToPage("Index");
    }
}
