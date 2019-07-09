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
        private readonly ILoginService _loginService;
        private readonly ISignUpService _signUpService;

        public LoginController(
            ILoginService loginService,
            ISignUpService signUpService)
        {
            _loginService = loginService;
            _signUpService = signUpService;
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public async Task<IActionResult> IndexAsync()
        {
            await HttpContext.SignOutAsync();
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

        public IActionResult SignUp()
        {
            SignUpUser user = new SignUpUser();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpUser user)
        {
            if (ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(user.ID) && !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Password) && !string.IsNullOrEmpty(user.CheckPassword))
                {
                    if(user.Password != user.CheckPassword)
                    {
                        ViewBag.ErrorInfo = "Created and confirmed passwords are not the same";
                        return View(user);
                    }
                    else if (!await _signUpService.CheckId(user.ID))
                    {
                        if (await _signUpService.SignUpAsync(user))
                        {
                            await HttpContext.SignOutAsync();

                            var claimIdentity = new ClaimsIdentity("Cookie");
                            claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.ID));
                            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                            claimIdentity.AddClaim(new Claim("Avatar", "Default.png"));
                            claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Common"));

                            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);

                            return RedirectToAction("Index", "Home");
                        }
                        else ViewBag.ErrorInfo = "注册过程存在异常，请进行调试";
                    }
                    else ViewBag.ErrorInfo = "UserId is exist";
                }
                else ViewBag.ErrorInfo = "Information is incomplete";
            }
            return View(user);
        }

        public IActionResult SignOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Welcome", "Login");
        }
    }
}