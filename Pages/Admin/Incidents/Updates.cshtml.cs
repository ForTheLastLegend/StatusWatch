using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Incidents;

[Authorize(Roles = "Admin")]
public class UpdatesModel : PageModel
{
    private readonly IncidentService _incidents;
    private readonly IncidentUpdateService _updates;

    public Incident Incident { get; set; } = default!;
    public List<IncidentUpdate> Updates { get; set; } = new();

    [BindProperty]
    public IncidentUpdate NewUpdate { get; set; } = new();

    public UpdatesModel(IncidentService incidents, IncidentUpdateService updates)
    {
        _incidents = incidents;
        _updates = updates;
    }

    public IActionResult OnGet(int id)
    {
        var i = _incidents.GetById(id);
        if (i == null)
        {
            return NotFound();
        }

        Incident = i;
        Updates = _updates.GetByIncident(id);
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var i = _incidents.GetById(id);
        if (i == null)
        {
            return NotFound();
        }

        // on ne valide que le champ Message — IncidentId est fixé côté serveur
        ModelState.Remove("NewUpdate.IncidentId");

        if (!ModelState.IsValid)
        {
            Incident = i;
            Updates = _updates.GetByIncident(id);
            return Page();
        }

        NewUpdate.IncidentId = id;
        _updates.Create(NewUpdate);
        TempData["Message"] = "Mise à jour publiée";
        return RedirectToPage(new { id });
    }
}
