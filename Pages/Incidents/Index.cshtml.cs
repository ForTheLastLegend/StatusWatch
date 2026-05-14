using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatusWatch.Models;
using StatusWatch.Services;

namespace StatusWatch.Pages.Incidents;

public class IndexModel : PageModel
{
    private readonly IncidentService _incidents;
    private const int PageSize = 10;

    public List<Incident> Incidents { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Statut { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Severite { get; set; }

    [BindProperty(SupportsGet = true, Name = "pagenumber")]
    public int CurrentPage { get; set; } = 1;

    public IndexModel(IncidentService incidents)
    {
        _incidents = incidents;
    }

    public void OnGet()
    {
        // page 1 minimum, sinon offset négatif côté SQL
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        TotalCount = _incidents.Count(Search, Statut, Severite);
        TotalPages = Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

        if (CurrentPage > TotalPages)
        {
            CurrentPage = TotalPages;
        }

        Incidents = _incidents.Search(Search, Statut, Severite, CurrentPage, PageSize);
    }
}
