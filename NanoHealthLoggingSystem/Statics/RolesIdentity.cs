using Microsoft.AspNetCore.Identity;
using NanoHealthLoggingSystem.Context;
using NanoHealthLoggingSystem.Enums;

namespace NanoHealthLoggingSystem.Statics
{
    public static class RolesIdentity
    {        
        public static async Task EnsureRolesAsync(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] {UserRole.User.ToString(), UserRole.Admin.ToString() };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
