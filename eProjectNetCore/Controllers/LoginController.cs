using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace eProjectNetCore.Controllers
{
    public class LoginController : Controller
    {
        AppDbContext _context;
        public LoginController(AppDbContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(String userName, String password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                ViewBag.error = "Tài khoản hoặc mật khẩu không để trống";
                return View("Index");
            }
            var md5pass = MD5Utils.MD5Hash(password);
            var acc = _context.Account.FirstOrDefault(x => x.UserName == userName && x.Password == md5pass);
            if (acc != null)
            {
                var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, userName)
                }, "BkapSecurityScheme");
                var pricipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync("BkapSecurityScheme", pricipal);
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.error = "Tài khoản hoặc mật khẩu không hợp lệ";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("BkapSecurityScheme");
            return RedirectToAction("Index", "Login");
        }
    }
}