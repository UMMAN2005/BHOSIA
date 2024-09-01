using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BHOSIA.BUSINESS.Helpers;

public static class DbInitializer {
  public static async Task SeedData(IServiceProvider serviceProvider) {
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();

    // Seed roles
    if (!await roleManager.RoleExistsAsync("Admin")) {
      await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("Member")) {
      await roleManager.CreateAsync(new IdentityRole("Member"));
    }
    if (!await roleManager.RoleExistsAsync("SuperAdmin")) {
      await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
    }

    // Seed users
    var user1 = new AppUser {
      Id = "custom-id-admin",
      UserName = "admin",
      Email = "admin@bhosia.com",
      EmailConfirmed = true
    };
    var user2 = new AppUser {
      Id = "custom-id-member",
      UserName = "member",
      Email = "member@bhosia.com",
      EmailConfirmed = true
    };
    var user3 = new AppUser {
      Id = "custom-id-superadmin",
      UserName = "superadmin",
      Email = "superadmin@bhosia.com",
      EmailConfirmed = true
    };

    if (userManager.Users.All(u => u.UserName != user1.UserName)) {
      await userManager.CreateAsync(user1, "Admin123");
      await userManager.AddToRoleAsync(user1, "Admin");
    }
    if (userManager.Users.All(u => u.UserName != user2.UserName)) {
      await userManager.CreateAsync(user2, "Member123");
      await userManager.AddToRoleAsync(user2, "Member");
    }
    if (userManager.Users.All(u => u.UserName != user3.UserName)) {
      await userManager.CreateAsync(user3, "SuperAdmin123");
      await userManager.AddToRoleAsync(user3, "SuperAdmin");
    }
  }
}