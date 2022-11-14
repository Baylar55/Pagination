using Fiorello.Constants;
using Fiorello.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorello.Controllers
{
    public class TempController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TempController(UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Test()
        {

            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name=role.ToString(),
                });
            }

            var user = new User
            {
                Fullname="admin",
                UserName = "admin",
                Email = "admin@app.com"
            };
            var result = await _userManager.CreateAsync(user, "Admin123*");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,UserRoles.Admin.ToString());
            }

            return View();
        }
    }
}
