using Ecommerce533.Migrations;
using Ecommerce533.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce533.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            //var userVM = new ApplicationUserVM();
            //userVM.Name = user.Name; 
            //userVM.Email = user.Email; 
            //userVM.Address= user.Address; 
            //userVM.Phone= user.PhoneNumber; 

            var userVM = user.Adapt<ApplicationUserVM>(); 

            return View(userVM);
        }
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = applicationUserVM.Name; 
            user.Address = applicationUserVM.Address; 
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            
            var result =  await _userManager.UpdateAsync(user); 
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors; 
            }
            else
            {
                TempData["Successful_Notification"] = "update user Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var result =  await _userManager.ChangePasswordAsync(user , applicationUserVM.CurrentPassword , applicationUserVM.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("," , result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update user Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
