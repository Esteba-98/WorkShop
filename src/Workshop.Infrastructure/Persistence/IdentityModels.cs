using Microsoft.AspNetCore.Identity;

namespace Workshop.Infrastructure.Persistence;

public class AppUser : IdentityUser<Guid>
{
    public string Nombre { get; set; } = "";
}

public class AppRole : IdentityRole<Guid>
{
}
