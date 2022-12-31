using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models.Models;
using HandmadeStore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository
{
    public class DbIntializer : IDbIntializer
    {
        private readonly UserManager<IdentityUser> userManger;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext dbContext;

        public DbIntializer(UserManager<IdentityUser> userManger, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            this.userManger = userManger;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }
        public async Task Intializer()
        {
            try
            {
                if (this.dbContext.Database.GetPendingMigrations().Any())
                {
                    this.dbContext.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (!await this.roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await this.roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await this.roleManager.CreateAsync(new IdentityRole(SD.Role_Shop));
                await this.roleManager.CreateAsync(new IdentityRole(SD.Role_Moderator));
                await this.roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            }

            var admin = new ApplicationUser
            {
                UserName = "admin@handmad.com",
                Email = "admin@handmad.com",
                Name = "Admin",
                City = "Cairo",
                StreetAdress = "Giza st",
                PhoneNumber = "01520436954",
                PostalCode = "7289"
            };
            await this.userManger.CreateAsync(admin,"Password1!");
            await this.userManger.AddToRoleAsync(admin,SD.Role_Admin);

        }
    }
}
