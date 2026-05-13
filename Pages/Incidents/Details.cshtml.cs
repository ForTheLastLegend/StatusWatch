using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Incidents;

public class DetailsModel : PageModel
{
    private readonly IncidentService _incidents;

    public Incident Incident { get; set; } = default!;

    public DetailsModel(IncidentService incidents)
    {
        _incidents = incidents;
    }

    public IActionResult OnGet(int id)
    {
        var incident = _incidents.GetById(id);
        if (incident == null)
        {
            return NotFound();
        }

        Incident = incident;
        return Page();
    }
}
