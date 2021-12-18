using mrs.Domain.Entities;
using mrs.Domain.ValueObjects;
using mrs.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System;
using mrs.Domain.Enums;

namespace mrs.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            var administratorRole1 = new ApplicationRole("1", "Account suspended", "{\"WF1\":\"4\",\"WF2\":\"4\",\"WF4\":\"4\",\"WF4.1\":\"4\",\"WF4.1.1\":\"4\",\"WF4.1.2\":\"4\",\"WF4.1.3\":\"4\",\"WF4.2\":\"4\",\"WF4.3\":\"4\",\"WF4.4\":\"4\",\"WF5\":\"4\",\"WF5.1.1\":\"4\",\"WF5.1.2\":\"4\",\"WF5.1.3\":\"4\",\"WF5.2\":\"4\",\"WF5.3\":\"4\",\"WF5.4\":\"4\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"4\",\"WF7.1\":\"4\",\"WF7.1.1\":\"4\",\"WF7.1.2\":\"4\",\"WF7.1.3\":\"4\",\"WF7.2\":\"4\",\"WF7.3\":\"4\",\"WF7.4\":\"4\",\"WF8\":\"4\",\"WF8.1\":\"4\",\"WF8.2\":\"4\",\"WF8.3\":\"4\",\"WF9\":\"4\",\"WF9.1\":\"4\",\"WF9.1.1\":\"4\",\"WF9.1.2\":\"4\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"4\",\"WF9.2\":\"4\",\"WF9.2.1\":\"4\",\"WF9.2.2\":\"4\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"4\",\"WF9.3\":\"4\",\"WF9.3.1\":\"4\",\"WF9.3.2\":\"4\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"4\",\"WF9.4\":\"4\",\"WF9.4.1\":\"4\",\"WF9.4.2\":\"4\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"4\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");
            var administratorRole2 = new ApplicationRole("2", "Level 2 app account", "{\"WF1\":\"4\",\"WF2\":\"4\",\"WF4\":\"4\",\"WF4.1\":\"4\",\"WF4.1.1\":\"4\",\"WF4.1.2\":\"4\",\"WF4.1.3\":\"4\",\"WF4.2\":\"4\",\"WF4.3\":\"4\",\"WF4.4\":\"4\",\"WF5\":\"4\",\"WF5.1.1\":\"4\",\"WF5.1.2\":\"4\",\"WF5.1.3\":\"4\",\"WF5.2\":\"4\",\"WF5.3\":\"4\",\"WF5.4\":\"4\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"4\",\"WF7.1\":\"4\",\"WF7.1.1\":\"4\",\"WF7.1.2\":\"4\",\"WF7.1.3\":\"4\",\"WF7.2\":\"4\",\"WF7.3\":\"4\",\"WF7.4\":\"4\",\"WF8\":\"4\",\"WF8.1\":\"4\",\"WF8.2\":\"4\",\"WF8.3\":\"4\",\"WF9\":\"4\",\"WF9.1\":\"4\",\"WF9.1.1\":\"4\",\"WF9.1.2\":\"4\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"4\",\"WF9.2\":\"4\",\"WF9.2.1\":\"4\",\"WF9.2.2\":\"4\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"4\",\"WF9.3\":\"4\",\"WF9.3.1\":\"4\",\"WF9.3.2\":\"4\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"4\",\"WF9.4\":\"4\",\"WF9.4.1\":\"4\",\"WF9.4.2\":\"4\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"4\",\"MF1.1\":\"1\",\"MF1.2\":\"1\",\"MF2\":\"3\",\"MF3\":\"3\",\"MF4\":\"3\",\"MF5\":\"3\",\"MF6\":\"3\",\"MF7\":\"3\",\"MF8\":\"3\",\"MF9\":\"3\",\"MF10\":\"3\",\"MF10.1\":\"3\",\"MF10.2\":\"3\",\"MF10.3\":\"3\",\"MF10.4\":\"3\",\"MF10.5\":\"3\",\"MF10.6\":\"3\",\"MF10.7\":\"3\"}");
            var administratorRole3 = new ApplicationRole("3", "Level 3 app account", "{\"WF1\":\"4\",\"WF2\":\"4\",\"WF4\":\"4\",\"WF4.1\":\"4\",\"WF4.1.1\":\"4\",\"WF4.1.2\":\"4\",\"WF4.1.3\":\"4\",\"WF4.2\":\"4\",\"WF4.3\":\"4\",\"WF4.4\":\"4\",\"WF5\":\"4\",\"WF5.1.1\":\"4\",\"WF5.1.2\":\"4\",\"WF5.1.3\":\"4\",\"WF5.2\":\"4\",\"WF5.3\":\"4\",\"WF5.4\":\"4\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"4\",\"WF7.1\":\"4\",\"WF7.1.1\":\"4\",\"WF7.1.2\":\"4\",\"WF7.1.3\":\"4\",\"WF7.2\":\"4\",\"WF7.3\":\"4\",\"WF7.4\":\"4\",\"WF8\":\"4\",\"WF8.1\":\"4\",\"WF8.2\":\"4\",\"WF8.3\":\"4\",\"WF9\":\"4\",\"WF9.1\":\"4\",\"WF9.1.1\":\"4\",\"WF9.1.2\":\"4\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"4\",\"WF9.2\":\"4\",\"WF9.2.1\":\"4\",\"WF9.2.2\":\"4\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"4\",\"WF9.3\":\"4\",\"WF9.3.1\":\"4\",\"WF9.3.2\":\"4\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"4\",\"WF9.4\":\"4\",\"WF9.4.1\":\"4\",\"WF9.4.2\":\"4\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"4\",\"MF1.1\":\"1\",\"MF1.2\":\"1\",\"MF2\":\"2\",\"MF3\":\"2\",\"MF4\":\"2\",\"MF5\":\"2\",\"MF6\":\"2\",\"MF7\":\"2\",\"MF8\":\"2\",\"MF9\":\"2\",\"MF10\":\"2\",\"MF10.1\":\"2\",\"MF10.2\":\"2\",\"MF10.3\":\"2\",\"MF10.4\":\"2\",\"MF10.5\":\"2\",\"MF10.6\":\"2\",\"MF10.7\":\"2\"}");
            var administratorRole6 = new ApplicationRole("6", "Company manager", "{\"WF1\":\"1\",\"WF2\":\"1\",\"WF4\":\"1\",\"WF4.1\":\"2\",\"WF4.1.1\":\"2\",\"WF4.1.2\":\"2\",\"WF4.1.3\":\"4\",\"WF4.2\":\"2\",\"WF4.3\":\"4\",\"WF4.4\":\"4\",\"WF5\":\"1\",\"WF5.1.1\":\"2\",\"WF5.1.2\":\"2\",\"WF5.1.3\":\"4\",\"WF5.2\":\"2\",\"WF5.3\":\"4\",\"WF5.4\":\"4\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"1\",\"WF7.1\":\"2\",\"WF7.1.1\":\"2\",\"WF7.1.2\":\"2\",\"WF7.1.3\":\"4\",\"WF7.2\":\"2\",\"WF7.3\":\"4\",\"WF7.4\":\"4\",\"WF8\":\"1\",\"WF8.1\":\"2\",\"WF8.2\":\"2\",\"WF8.3\":\"2\",\"WF9\":\"1\",\"WF9.1\":\"4\",\"WF9.1.1\":\"4\",\"WF9.1.2\":\"4\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"4\",\"WF9.2\":\"1\",\"WF9.2.1\":\"2\",\"WF9.2.2\":\"2\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"2\",\"WF9.3\":\"1\",\"WF9.3.1\":\"2\",\"WF9.3.2\":\"2\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"2\",\"WF9.4\":\"1\",\"WF9.4.1\":\"2\",\"WF9.4.2\":\"2\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"2\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");
            var administratorRole7 = new ApplicationRole("7", "RPA user", "{\"WF1\":\"1\",\"WF2\":\"1\",\"WF4\":\"1\",\"WF4.1\":\"1\",\"WF4.1.1\":\"1\",\"WF4.1.2\":\"1\",\"WF4.1.3\":\"1\",\"WF4.2\":\"1\",\"WF4.3\":\"4\",\"WF4.4\":\"4\",\"WF5\":\"1\",\"WF5.1.1\":\"1\",\"WF5.1.2\":\"1\",\"WF5.1.3\":\"1\",\"WF5.2\":\"1\",\"WF5.3\":\"4\",\"WF5.4\":\"4\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"1\",\"WF7.1\":\"1\",\"WF7.1.1\":\"1\",\"WF7.1.2\":\"1\",\"WF7.1.3\":\"1\",\"WF7.2\":\"1\",\"WF7.3\":\"4\",\"WF7.4\":\"4\",\"WF8\":\"1\",\"WF8.1\":\"1\",\"WF8.2\":\"1\",\"WF8.3\":\"1\",\"WF9\":\"4\",\"WF9.1\":\"4\",\"WF9.1.1\":\"4\",\"WF9.1.2\":\"4\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"4\",\"WF9.2\":\"4\",\"WF9.2.1\":\"4\",\"WF9.2.2\":\"4\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"4\",\"WF9.3\":\"4\",\"WF9.3.1\":\"4\",\"WF9.3.2\":\"4\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"4\",\"WF9.4\":\"4\",\"WF9.4.1\":\"4\",\"WF9.4.2\":\"4\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"4\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");
            var administratorRole8 = new ApplicationRole("8", "General user", "{\"WF1\":\"1\",\"WF2\":\"1\",\"WF4\":\"1\",\"WF4.1\":\"1\",\"WF4.1.1\":\"1\",\"WF4.1.2\":\"1\",\"WF4.1.3\":\"1\",\"WF4.2\":\"1\",\"WF4.3\":\"1\",\"WF4.4\":\"1\",\"WF5\":\"1\",\"WF5.1.1\":\"1\",\"WF5.1.2\":\"1\",\"WF5.1.3\":\"1\",\"WF5.2\":\"1\",\"WF5.3\":\"1\",\"WF5.4\":\"1\",\"WF6\":\"4\",\"WF6.1\":\"4\",\"WF6.2\":\"4\",\"WF6.3\":\"4\",\"WF6.4\":\"4\",\"WF7\":\"1\",\"WF7.1\":\"1\",\"WF7.1.1\":\"1\",\"WF7.1.2\":\"1\",\"WF7.1.3\":\"1\",\"WF7.2\":\"1\",\"WF7.3\":\"1\",\"WF7.4\":\"1\",\"WF8\":\"1\",\"WF8.1\":\"1\",\"WF8.2\":\"1\",\"WF8.3\":\"1\",\"WF9\":\"1\",\"WF9.1\":\"1\",\"WF9.1.1\":\"1\",\"WF9.1.2\":\"1\",\"WF9.1.3\":\"4\",\"WF9.1.4\":\"4\",\"WF9.1.5\":\"1\",\"WF9.2\":\"1\",\"WF9.2.1\":\"1\",\"WF9.2.2\":\"1\",\"WF9.2.3\":\"4\",\"WF9.2.4\":\"4\",\"WF9.2.5\":\"1\",\"WF9.3\":\"1\",\"WF9.3.1\":\"1\",\"WF9.3.2\":\"1\",\"WF9.3.3\":\"4\",\"WF9.3.4\":\"4\",\"WF9.3.5\":\"1\",\"WF9.4\":\"1\",\"WF9.4.1\":\"1\",\"WF9.4.2\":\"1\",\"WF9.4.3\":\"4\",\"WF9.4.4\":\"4\",\"WF9.4.5\":\"1\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");
            var administratorRole9 = new ApplicationRole("9", "Master data manager", "{\"WF1\":\"1\",\"WF2\":\"1\",\"WF4\":\"1\",\"WF4.1\":\"1\",\"WF4.1.1\":\"1\",\"WF4.1.2\":\"1\",\"WF4.1.3\":\"1\",\"WF4.2\":\"1\",\"WF4.3\":\"1\",\"WF4.4\":\"1\",\"WF5\":\"1\",\"WF5.1.1\":\"1\",\"WF5.1.2\":\"1\",\"WF5.1.3\":\"1\",\"WF5.2\":\"1\",\"WF5.3\":\"1\",\"WF5.4\":\"1\",\"WF6\":\"1\",\"WF6.1\":\"1\",\"WF6.2\":\"1\",\"WF6.3\":\"1\",\"WF6.4\":\"1\",\"WF7\":\"1\",\"WF7.1\":\"1\",\"WF7.1.1\":\"1\",\"WF7.1.2\":\"1\",\"WF7.1.3\":\"1\",\"WF7.2\":\"1\",\"WF7.3\":\"1\",\"WF7.4\":\"1\",\"WF8\":\"1\",\"WF8.1\":\"1\",\"WF8.2\":\"1\",\"WF8.3\":\"1\",\"WF9\":\"1\",\"WF9.1\":\"1\",\"WF9.1.1\":\"1\",\"WF9.1.2\":\"1\",\"WF9.1.3\":\"1\",\"WF9.1.4\":\"1\",\"WF9.1.5\":\"1\",\"WF9.2\":\"1\",\"WF9.2.1\":\"1\",\"WF9.2.2\":\"1\",\"WF9.2.3\":\"1\",\"WF9.2.4\":\"1\",\"WF9.2.5\":\"1\",\"WF9.3\":\"1\",\"WF9.3.1\":\"1\",\"WF9.3.2\":\"1\",\"WF9.3.3\":\"1\",\"WF9.3.4\":\"1\",\"WF9.3.5\":\"1\",\"WF9.4\":\"1\",\"WF9.4.1\":\"1\",\"WF9.4.2\":\"1\",\"WF9.4.3\":\"1\",\"WF9.4.4\":\"1\",\"WF9.4.5\":\"1\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");
            var administratorRole10 = new ApplicationRole("10", "System admin", "{\"WF1\":\"1\",\"WF2\":\"1\",\"WF4\":\"1\",\"WF4.1\":\"1\",\"WF4.1.1\":\"1\",\"WF4.1.2\":\"1\",\"WF4.1.3\":\"1\",\"WF4.2\":\"1\",\"WF4.3\":\"1\",\"WF4.4\":\"1\",\"WF5\":\"1\",\"WF5.1.1\":\"1\",\"WF5.1.2\":\"1\",\"WF5.1.3\":\"1\",\"WF5.2\":\"1\",\"WF5.3\":\"1\",\"WF5.4\":\"1\",\"WF6\":\"1\",\"WF6.1\":\"1\",\"WF6.2\":\"1\",\"WF6.3\":\"1\",\"WF6.4\":\"1\",\"WF7\":\"1\",\"WF7.1\":\"1\",\"WF7.1.1\":\"1\",\"WF7.1.2\":\"1\",\"WF7.1.3\":\"1\",\"WF7.2\":\"1\",\"WF7.3\":\"1\",\"WF7.4\":\"1\",\"WF8\":\"1\",\"WF8.1\":\"1\",\"WF8.2\":\"1\",\"WF8.3\":\"1\",\"WF9\":\"1\",\"WF9.1\":\"1\",\"WF9.1.1\":\"1\",\"WF9.1.2\":\"1\",\"WF9.1.3\":\"1\",\"WF9.1.4\":\"1\",\"WF9.1.5\":\"1\",\"WF9.2\":\"1\",\"WF9.2.1\":\"1\",\"WF9.2.2\":\"1\",\"WF9.2.3\":\"1\",\"WF9.2.4\":\"1\",\"WF9.2.5\":\"1\",\"WF9.3\":\"1\",\"WF9.3.1\":\"1\",\"WF9.3.2\":\"1\",\"WF9.3.3\":\"1\",\"WF9.3.4\":\"1\",\"WF9.3.5\":\"1\",\"WF9.4\":\"1\",\"WF9.4.1\":\"1\",\"WF9.4.2\":\"1\",\"WF9.4.3\":\"1\",\"WF9.4.4\":\"1\",\"WF9.4.5\":\"1\",\"MF1.1\":\"4\",\"MF1.2\":\"4\",\"MF2\":\"4\",\"MF3\":\"4\",\"MF4\":\"4\",\"MF5\":\"4\",\"MF6\":\"4\",\"MF7\":\"4\",\"MF8\":\"4\",\"MF9\":\"4\",\"MF10\":\"4\",\"MF10.1\":\"4\",\"MF10.2\":\"4\",\"MF10.3\":\"4\",\"MF10.4\":\"4\",\"MF10.5\":\"4\",\"MF10.6\":\"4\",\"MF10.7\":\"4\"}");

            if (roleManager.Roles.All(r => r.Name != administratorRole1.Name))
            {
                await roleManager.CreateAsync(administratorRole1);
                await roleManager.CreateAsync(administratorRole2);
                await roleManager.CreateAsync(administratorRole3);
                await roleManager.CreateAsync(administratorRole6);
                await roleManager.CreateAsync(administratorRole7);
                await roleManager.CreateAsync(administratorRole8);
                await roleManager.CreateAsync(administratorRole9);
                await roleManager.CreateAsync(administratorRole10);
            }

            var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost", CreatedAt = DateTime.Now };
            var userRole1 = new ApplicationUser { UserName = "user1", Email = "user1@localhost", CreatedAt = DateTime.Now };
            var userRole2 = new ApplicationUser { UserName = "user2", Email = "user2@localhost", CreatedAt = DateTime.Now };
            var userRole3 = new ApplicationUser { UserName = "user3", Email = "user3@localhost", CreatedAt = DateTime.Now };
            var userRole6 = new ApplicationUser { UserName = "user6", Email = "user6@localhost", CreatedAt = DateTime.Now };
            var userRole7 = new ApplicationUser { UserName = "user7", Email = "user7@localhost", CreatedAt = DateTime.Now };
            var userRole8 = new ApplicationUser { UserName = "user8", Email = "user8@localhost", CreatedAt = DateTime.Now };
            var userRole9 = new ApplicationUser { UserName = "user9", Email = "user9@localhost", CreatedAt = DateTime.Now };
            var userRole10 = new ApplicationUser { UserName = "user10", Email = "user10@localhost", CreatedAt = DateTime.Now };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Administrator1!");
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole3.Name });
                await userManager.CreateAsync(userRole1, "Administrator1!");
                await userManager.AddToRolesAsync(userRole1, new[] { administratorRole1.Name });
                await userManager.CreateAsync(userRole2, "Administrator1!");
                await userManager.AddToRolesAsync(userRole2, new[] { administratorRole2.Name });
                await userManager.CreateAsync(userRole3, "Administrator1!");
                await userManager.AddToRolesAsync(userRole3, new[] { administratorRole3.Name });
                await userManager.CreateAsync(userRole6, "Administrator1!");
                await userManager.AddToRolesAsync(userRole6, new[] { administratorRole6.Name });
                await userManager.CreateAsync(userRole7, "Administrator1!");
                await userManager.AddToRolesAsync(userRole7, new[] { administratorRole7.Name });
                await userManager.CreateAsync(userRole8, "Administrator1!");
                await userManager.AddToRolesAsync(userRole8, new[] { administratorRole8.Name });
                await userManager.CreateAsync(userRole9, "Administrator1!");
                await userManager.AddToRolesAsync(userRole9, new[] { administratorRole9.Name });
                await userManager.CreateAsync(userRole10, "Administrator1!");
                await userManager.AddToRolesAsync(userRole10, new[] { administratorRole10.Name });
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (!context.RequestTypes.Any())
            {
                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.New.ToString(),
                    RequestTypeName = RequestTypeEnum.New.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.Switch.ToString(),
                    RequestTypeName = RequestTypeEnum.Switch.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.ReIssued.ToString(),
                    RequestTypeName = RequestTypeEnum.ReIssued.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.ChangeCard.ToString(),
                    RequestTypeName = RequestTypeEnum.ChangeCard.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.LeaveGroup.ToString(),
                    RequestTypeName = RequestTypeEnum.LeaveGroup.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.PMigrate.ToString(),
                    RequestTypeName = RequestTypeEnum.PMigrate.GetStringValue(),
                });

                context.RequestTypes.Add(new RequestType
                {
                    RequestTypeCode = RequestTypeEnum.Kid.ToString(),
                    RequestTypeName = RequestTypeEnum.Kid.GetStringValue(),
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
