using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Incidents;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly IncidentService _incidents;
    private readonly ServiceService _services;

    [BindProperty]
    public Incident Incident { get; set; } = new();

    public List<SelectListItem> ServiceOptions { get; set; } = new();

    public List<SelectListItem> StatutOptions { get; } = new()
    {
        new("investigating", "investigating"),
        new("monitoring", "monitoring"),
        new("resolved", "resolved")
    };

    public List<SelectListItem> SeveriteOptions { get; } = new()
    {
        new("minor", "minor"),
        new("major", "major"),
        new("critical", "critical")
    };

    public EditModel(IncidentService incidents, ServiceService services)
    {
        _incidents = incidents;
        _services = services;
    }

    public IActionResult OnGet(int id)
    {
        var i = _incidents.GetById(id);
        if (i == null)
        {
            return NotFound();
        }

        Incident = i;
        LoadOptions();
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        if (!ModelState.IsValid)
        {
            LoadOptions();
            return Page();
        }

        Incident.Id = id;
        _incidents.Update(Incident);
        TempData["Message"] = $"Incident '{Incident.Titre}' modifié";
        return RedirectToPage("/Incidents/Index");
    }

    private void LoadOptions()
    {
        ServiceOptions = _services.GetAll()
            .Select(s => new SelectListItem(s.Nom, s.Id.ToString()))
            .ToList();
    }
}
