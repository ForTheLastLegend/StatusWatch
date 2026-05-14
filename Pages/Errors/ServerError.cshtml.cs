using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StatusWatch.Pages.Errors;

[AllowAnonymous]
public class ServerErrorModel : PageModel
{
    public void OnGet()
    {
        Response.StatusCode = 500;
    }
}
