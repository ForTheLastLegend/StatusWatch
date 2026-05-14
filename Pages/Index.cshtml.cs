using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages;

public class IndexModel : PageModel
{
    private readonly ServiceService _services;
    private readonly IncidentService _incidents;
    private readonly PingService _pings;

    public List<Service> Services { get; set; } = new();
    public List<Incident> ActiveIncidents { get; set; } = new();
    public double? UptimeAverage { get; set; }

    public IndexModel(ServiceService services, IncidentService incidents, PingService pings)
    {
        _services = services;
        _incidents = incidents;
        _pings = pings;
    }

    public void OnGet()
    {
        Services = _services.GetAll();
        ActiveIncidents = _incidents.GetActive();

        // moyenne d'uptime sur 30j, on ignore les services sans ping
        var rates = Services
            .Select(s => _pings.GetUptimePercent(s.Id, 30))
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToList();
        UptimeAverage = rates.Count > 0 ? Math.Round(rates.Average(), 2) : null;
    }
}
