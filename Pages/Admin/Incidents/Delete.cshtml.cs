using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Admin.Incidents;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly IncidentService _incidents;

    public Incident Incident { get; set; } = default!;

    public DeleteModel(IncidentService incidents)
    {
        _incidents = incidents;
    }

    public IActionResult OnGet(int id)
    {
        var i = _incidents.GetById(id);
        if (i == null)
        {
            return NotFound();
        }

        Incident = i;
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var i = _incidents.GetById(id);
        if (i == null)
        {
            return NotFound();
        }

        _incidents.Delete(id);
        TempData["Message"] = $"Incident '{i.Titre}' supprimé";
        return RedirectToPage("/Incidents/Index");
    }
}
