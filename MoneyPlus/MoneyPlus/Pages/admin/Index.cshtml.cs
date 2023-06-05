namespace MoneyPlus.Pages.admin;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }

    [BindProperty]
    public string? pwd { get; set; }
}