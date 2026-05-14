using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Services;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ServiceService _services;
    private readonly IncidentService _incidents;

    public Service Service { get; set; } = default!;
    public bool HasActiveIncidents { get; set; }

    public DeleteModel(ServiceService services, IncidentService incidents)
    {
        _services = services;
        _incidents = incidents;
    }

    public IActionResult OnGet(int id)
    {
        var s = _services.GetById(id);
        if (s == null)
        {
            return NotFound();
        }

        Service = s;
        HasActiveIncidents = _incidents.HasActiveIncidents(id);
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var s = _services.GetById(id);
        if (s == null)
        {
            return NotFound();
        }

        if (_incidents.HasActiveIncidents(id))
        {
            Service = s;
            HasActiveIncidents = true;
            TempData["Error"] = $"Impossible de supprimer '{s.Nom}' : des incidents actifs y sont rattachés.";
            return Page();
        }

        _services.Delete(id);
        TempData["Message"] = $"Service '{s.Nom}' supprimé";
        return RedirectToPage("Index");
    }
}
