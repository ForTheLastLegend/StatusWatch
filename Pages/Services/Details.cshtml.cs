using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Services;

public class DetailsModel : PageModel
{
    private readonly ServiceService _services;

    public Service Service { get; set; } = default!;

    public DetailsModel(ServiceService services)
    {
        _services = services;
    }

    public IActionResult OnGet(int id)
    {
        var service = _services.GetById(id);
        if (service == null)
        {
            return NotFound();
        }

        Service = service;
        return Page();
    }
}
