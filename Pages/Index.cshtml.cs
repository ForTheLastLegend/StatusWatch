using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages;

public class IndexModel : PageModel
{
    private readonly ServiceService _services;
    private readonly IncidentService _incidents;

    public List<Service> Services { get; set; } = new();
    public List<Incident> ActiveIncidents { get; set; } = new();

    public IndexModel(ServiceService services, IncidentService incidents)
    {
        _services = services;
        _incidents = incidents;
    }

    public void OnGet()
    {
        Services = _services.GetAll();
        ActiveIncidents = _incidents.GetActive();
    }
}
