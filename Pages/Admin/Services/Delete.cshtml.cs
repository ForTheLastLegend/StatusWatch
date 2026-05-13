using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Services;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ServiceService _services;

    public Service Service { get; set; } = default!;

    public DeleteModel(ServiceService services)
    {
        _services = services;
    }

    public IActionResult OnGet(int id)
    {
        var s = _services.GetById(id);
        if (s == null)
        {
            return NotFound();
        }

        Service = s;
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var s = _services.GetById(id);
        if (s == null)
        {
            return NotFound();
        }

        _services.Delete(id);
        TempData["Message"] = $"Service '{s.Nom}' supprimé";
        return RedirectToPage("Index");
    }
}
