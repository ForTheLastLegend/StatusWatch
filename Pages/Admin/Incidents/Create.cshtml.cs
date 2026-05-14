using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Incidents;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IncidentService _incidents;
    private readonly ServiceService _services;

    [BindProperty]
    public Incident Incident { get; set; } = new();

    public List<SelectListItem> ServiceOptions { get; set; } = new();

    public List<SelectListItem> StatutOptions { get; } = new()
    {
        new("investigating", "En investigation"),
        new("identified", "Identifié"),
        new("monitoring", "Surveillance"),
        new("resolved", "Résolu")
    };

    public List<SelectListItem> SeveriteOptions { get; } = new()
    {
        new("minor", "Mineur"),
        new("major", "Majeur"),
        new("critical", "Critique")
    };

    public CreateModel(IncidentService incidents, ServiceService services)
    {
        _incidents = incidents;
        _services = services;
    }

    public void OnGet()
    {
        LoadOptions();
        Incident.DateDebut = DateTime.Now;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadOptions();
            return Page();
        }

        _incidents.Create(Incident);
        TempData["Message"] = $"Incident '{Incident.Titre}' créé";
        return RedirectToPage("/Incidents/Index");
    }

    private void LoadOptions()
    {
        ServiceOptions = _services.GetAll()
            .Select(s => new SelectListItem(s.Nom, s.Id.ToString()))
            .ToList();
    }
}
