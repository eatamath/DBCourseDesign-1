using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using No9Gallery.Models;
using No9Gallery.Services;

namespace No9Gallery.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginServiceInterface _loginService;

        public LoginController(
            ILoginServiceInterface loginService)
        {
            _loginService = loginService;
        }

        public IActionResult Index()
        {
            LoginUser user = new LoginUser();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginUser user)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(user.ID) && !string.IsNullOrEmpty(user.Password))
            {
                var getUser = await _loginService.CheckLogin(user.ID, user.Password);

                if (getUser != null)
                {
                    var claimIdentity = new ClaimsIdentity("Cookie");
                    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, getUser.ID));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, getUser.Name));
                    claimIdentity.AddClaim(new Claim("Avatar", getUser.Avatar));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, getUser.Status));

                    var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ErrorInfo = "UserId or password is wrong";
                return View(user);
            }
            else if (string.IsNullOrEmpty(user.ID))
            {
                ViewBag.ErrorInfo = "UserId is empty";
            }
            else ViewBag.ErrorInfo = "Password is empty";
            return View(user);
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Welcome", "Login");
        }
    }
}