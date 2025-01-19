using Gaming.BL.VM.Account;
using Gaming.Core.Entities;
using Gaming.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gaming.MVC.Controllers;
public class AccountController : Controller
{
    readonly SignInManager<User> _signInManager;
    readonly UserManager<User> _userManager;

    bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false; 

    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager; 
        _userManager= userManager;
    }

    public ActionResult Register()
    {
        if (IsAuthenticated) return RedirectToAction("Index", "Home"); 
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (IsAuthenticated) return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid) return View();

        if(vm.Password != vm.RePassword)
        {
            ModelState.AddModelError("", "Passwords do not match");
            return View(); 
        }

        User user = new User
        {
            Fullname = vm.Fullname,
            Email = vm.Email,
            UserName = vm.Username
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if(!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(); 
        }

        var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.Admin));

        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }
        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login()
    {
        return View(); 
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string returnUrl = null)
    {
        if (IsAuthenticated) return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid) return View();

        User user = null;

        if (vm.EmailOrUsername.Contains('@'))
            user = await _userManager.FindByEmailAsync(vm.EmailOrUsername);
        else
            user = await _userManager.FindByNameAsync(vm.EmailOrUsername);

        if(user == null)
        {
            ModelState.AddModelError("", "Username or password wrong");
            return View(); 
        }

        var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);

        if (!result.Succeeded)
        {
            if (result.IsNotAllowed)
                ModelState.AddModelError("", "Username or password wrong");
            if (result.IsLockedOut)
                ModelState.AddModelError("", $"Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            return View(); 
        }

        if(string.IsNullOrWhiteSpace(returnUrl))
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });

            return RedirectToAction("Index", "Home");
        }
        return LocalRedirect(returnUrl); 
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
