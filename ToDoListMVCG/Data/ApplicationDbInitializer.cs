using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListMVCG.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.FindByNameAsync("Admin").Result == null)
            {
                //the role does not exist and we have to create it!
                roleManager.CreateAsync(
                    new IdentityRole()
                    {
                        Name = "Admin"
                    }
                ).Wait();
            }

            if (roleManager.FindByNameAsync("RegularUser").Result == null)
            {
                //the role does not exist and we have to create it!
                roleManager.CreateAsync(
                    new IdentityRole()
                    {
                        Name = "RegularUser"
                    }
                ).Wait();
            }

            if(userManager.FindByNameAsync("admin").Result == null)
            {
                //the user does not exist
                AppUser user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                IdentityResult result =
                    userManager.CreateAsync(user, "adminpassword").Result;
                if(result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
