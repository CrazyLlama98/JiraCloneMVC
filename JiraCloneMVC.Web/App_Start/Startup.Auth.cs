using JiraCloneMVC.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartup(typeof(JiraCloneMVC.Web.App_Start.Startup))]

namespace JiraCloneMVC.Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            CreateAdminAccountAndApplicationRoles();
        }

        private void CreateAdminAccountAndApplicationRoles()
        {
            var dbContext = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<User>(dbContext));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(dbContext));

            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Administrator"
                });

                var user = new User
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };
                var adminCreated = userManager.Create(user, "123admin123");
                if (adminCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }

            if (!roleManager.RoleExists("Organizator"))
                roleManager.Create(new IdentityRole
                {
                    Name = "Organizator"
                });

            if (!roleManager.RoleExists("Member"))
                roleManager.Create(new IdentityRole
                {
                    Name = "Member"
                });            if (!roleManager.RoleExists("User"))
                roleManager.Create(new IdentityRole
                {
                    Name = "User"
                });
        }
    }
}
