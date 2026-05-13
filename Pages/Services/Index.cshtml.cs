using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Services;

public class IndexModel : PageModel
{
    private readonly ServiceService _services;

    public List<Service> Services { get; set; } = new();

    public IndexModel(ServiceService services)
    {
        _services = services;
    }

    public void OnGet()
    {
        Services = _services.GetAll();
    }
}
