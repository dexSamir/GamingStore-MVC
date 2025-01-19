using Gaming.Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gaming.Core.Enums;

namespace Gaming.BL.Extension;
public static class UserSeedExtension
{
	public static async void UseUserSeed(this IApplicationBuilder app)
	{
		using(var scope = app.ApplicationServices.CreateScope())
		{
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			if (!roleManager.Roles.Any())
				foreach (var item in Enum.GetValues(typeof(Roles)))
					await roleManager.CreateAsync(new IdentityRole(item.ToString())); 

			if(!userManager.Users.Any(x=> x.NormalizedUserName == "ADMIN"))
			{
				User user = new User
				{
					Fullname = "admin",
					Email = "admin@gmail.com",
					UserName = "admin"
				};

				await userManager.CreateAsync(user, "admin123");
				await userManager.AddToRoleAsync(user, nameof(Roles.Admin));
			}

		}
	}
}

