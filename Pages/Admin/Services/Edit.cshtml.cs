using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Services;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
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

    public EditModel(ServiceService services)
    {
        _services = services;
    }

    public IActionResult OnGet(int id)
    {
        var s = _services.GetById(id);
        if (s == null)
        {
            return NotFound();
        }

        Service = s;
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Service.Id = id;
        _services.Update(Service);
        TempData["Message"] = $"Service '{Service.Nom}' modifié.";
        return RedirectToPage("Index");
    }
}
