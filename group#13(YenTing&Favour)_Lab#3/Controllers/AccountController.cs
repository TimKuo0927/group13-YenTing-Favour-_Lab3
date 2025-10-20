using group_13_YenTing_Favour__Lab_3.Models;
using group_13_YenTing_Favour__Lab_3.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace group_13_YenTing_Favour__Lab_3.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Lab3Context _db;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, Lab3Context db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                   
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "wrong email or password");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // add new user to db
                    await _userManager.AddToRoleAsync(user, model.Role);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }



        //// ---------- LOGOUT ----------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UserSubPage()
        {
            string userId = _userManager.GetUserId(User);
            var SubList = _db.Subscriptions
                            .Where(s => s.UserId == userId)
                            .OrderByDescending(s => s.SubscribedDate)
                            .Include(x=>x.Podcast)
                            .ToList();
            return View(SubList);
        }

    }
}
