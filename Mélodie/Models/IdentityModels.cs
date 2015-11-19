using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Mélodie.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }
        public string Role_id { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            //: base("DefaultConnection")
            : base("MélodieContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            // Keep this:
            modelBuilder.Entity<IdentityUser>().ToTable("Users");

            // Change TUser to ApplicationUser everywhere else - 
            // IdentityUser and ApplicationUser essentially 'share' the AspNetUsers Table in the database:
            EntityTypeConfiguration<ApplicationUser> table =
                modelBuilder.Entity<ApplicationUser>().ToTable("Users");

            table.Property((ApplicationUser u) => u.UserName).IsRequired();

            // EF won't let us swap out IdentityUserRole for ApplicationUserRole here:
            modelBuilder.Entity<ApplicationUser>().HasMany<IdentityUserRole>((ApplicationUser u) => u.Roles);
            modelBuilder.Entity<IdentityUserRole>().HasKey((IdentityUserRole r) =>
                new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");

            // Leave this alone:
            EntityTypeConfiguration<IdentityUserLogin> entityTypeConfiguration =
                modelBuilder.Entity<IdentityUserLogin>().HasKey((IdentityUserLogin l) =>
                    new {
                        UserId = l.UserId,
                        LoginProvider = l.LoginProvider,
                        ProviderKey
                        = l.ProviderKey
                    }).ToTable("AspNetUserLogins");

            entityTypeConfiguration.HasRequired<IdentityUser>((IdentityUserLogin u) => u.User);
            EntityTypeConfiguration<IdentityUserClaim> table1 =
                modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

            table1.HasRequired<IdentityUser>((IdentityUserClaim u) => u.User);

            // Add this, so that IdentityRole can share a table with ApplicationRole:
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");

            // Change these from IdentityRole to ApplicationRole:
            EntityTypeConfiguration<ApplicationRole> entityTypeConfiguration1 =
                modelBuilder.Entity<ApplicationRole>().ToTable("Roles");

            entityTypeConfiguration1.Property((ApplicationRole r) => r.Name).IsRequired();
        }
    }

    public class IdentityManager
    {
        public bool RoleExists(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.RoleExists(name);
        }


        public bool CreateRole(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = um.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                um.RemoveFromRole(userId, role.Role.Name);
            }
        }
    }
}
