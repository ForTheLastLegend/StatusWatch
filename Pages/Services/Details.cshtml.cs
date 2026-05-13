using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Services;

public class DetailsModel : PageModel
{
    private readonly ServiceService _services;
    private readonly IncidentService _incidents;

    public Service Service { get; set; } = default!;
    public List<Incident> Incidents { get; set; } = new();

    public DetailsModel(ServiceService services, IncidentService incidents)
    {
        _services = services;
        _incidents = incidents;
    }

    public IActionResult OnGet(int id)
    {
        var service = _services.GetById(id);
        if (service == null)
        {
            return NotFound();
        }

        Service = service;
        Incidents = _incidents.GetByService(id);
        return Page();
    }
}
