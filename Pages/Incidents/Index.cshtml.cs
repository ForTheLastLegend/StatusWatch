using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Incidents;

public class IndexModel : PageModel
{
    private readonly IncidentService _incidents;

    public List<Incident> Incidents { get; set; } = new();

    public IndexModel(IncidentService incidents)
    {
        _incidents = incidents;
    }

    public void OnGet()
    {
        Incidents = _incidents.GetAll();
    }
}
