namespace MoneyPlus.Pages.admin;

[AllowAnonymous]
public class secretModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public secretModel(ApplicationDbContext context,
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    [EmailAddress]
    public string Email { get; set; }
    [BindProperty]
    public string Password { get; set; }

    public IActionResult OnGet(string? pwd)
    {
        if (pwd == "pouco-segura")
        {
            return Page();
        }
        else
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await AssignAdminRoleAsync();
            return RedirectToPage(".././Index");
        }
        catch (Exception e)
        {
            return Page();
        }
    }

    public async Task AssignAdminRoleAsync()
    {
        _context.Database.EnsureCreated();

        string adminRole = "Admin";

        if (await _roleManager.FindByNameAsync(adminRole) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var registeredUsers = _context.Users.ToList();

        var userIfExists = registeredUsers.FirstOrDefault(r => r.Email == Email);

        if (userIfExists != null)
        {
            IdentityResult roleresult = await _userManager.AddToRoleAsync(userIfExists, adminRole);
            await _signInManager.SignInAsync(userIfExists, isPersistent: true);
        }
        else
        {
            var newUser = new IdentityUser
            {
                UserName = Email,
                Email = Email
            };

            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                await _userManager.AddPasswordAsync(newUser, Password);
                await _userManager.AddToRoleAsync(newUser, adminRole);
                await _signInManager.SignInAsync(newUser, isPersistent: true);
            }
        }
        await _context.SaveChangesAsync();
    }
}