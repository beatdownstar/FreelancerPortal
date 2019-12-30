using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{

    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string projectManagerEmail = "projectManager@gmail.com";
            string projectManagerpassword = "_Aa123456";

            string seniorDeveloperEmail = "seniorDeveloper@gmail.com";
            string seniorDeveloperpassword = "_Aa123456";

            if (await roleManager.FindByNameAsync("ProjectManager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("ProjectManager"));
            }
            if (await roleManager.FindByNameAsync("SeniorDeveloper") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("SeniorDeveloper"));
            }
            if (await roleManager.FindByNameAsync("Client") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Client"));
            }

            if (await roleManager.FindByNameAsync("Frilanser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Frilanser"));
            }

            if (await roleManager.FindByNameAsync("Java (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Java (Specialization)"));
            }

            if (await roleManager.FindByNameAsync("PHP (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("PHP (Specialization)"));
            }

            if (await roleManager.FindByNameAsync("WEB (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("WEB (Specialization)"));
            }

            if (await roleManager.FindByNameAsync(".NET (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(".NET (Specialization)"));
            }

            if (await roleManager.FindByNameAsync("C# (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("C# (Specialization)"));
            }

            if (await roleManager.FindByNameAsync("Universal (Specialization)") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Universal (Specialization)"));
            }
            
            if (await userManager.FindByNameAsync(projectManagerEmail) == null)
            {
                IdentityUser projectManager = new IdentityUser { Email = projectManagerEmail, UserName = projectManagerEmail };
                IdentityResult result = await userManager.CreateAsync(projectManager, projectManagerpassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(projectManager, "ProjectManager");
                }
            }

            if (await userManager.FindByNameAsync(seniorDeveloperEmail) == null)
            {
                IdentityUser seniorDeveloper = new IdentityUser { Email = seniorDeveloperEmail, UserName = seniorDeveloperEmail };
                IdentityResult result = await userManager.CreateAsync(seniorDeveloper, seniorDeveloperpassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(seniorDeveloper, "SeniorDeveloper");
                }
            }
        }
    }
}