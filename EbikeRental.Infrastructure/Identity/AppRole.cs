using Microsoft.AspNetCore.Identity;

namespace EbikeRental.Infrastructure.Identity;

public class AppRole : IdentityRole<int>
{
    public AppRole() : base() { }
    
    public AppRole(string roleName) : base(roleName) { }
}
