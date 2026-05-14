using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Incidents;

public class DetailsModel : PageModel
{
    private readonly IncidentService _incidents;
    private readonly IncidentUpdateService _updates;

    public Incident Incident { get; set; } = default!;
    public List<IncidentUpdate> Updates { get; set; } = new();

    public DetailsModel(IncidentService incidents, IncidentUpdateService updates)
    {
        _incidents = incidents;
        _updates = updates;
    }

    public IActionResult OnGet(int id)
    {
        var incident = _incidents.GetById(id);
        if (incident == null)
        {
            return NotFound();
        }

        Incident = incident;
        Updates = _updates.GetByIncident(id);
        return Page();
    }
}
