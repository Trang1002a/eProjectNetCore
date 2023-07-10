using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Models;
using eProjectNetCore.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        AppDbContext db;
        public LoginController(AppDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(String userName, String password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                ViewBag.error = "Tài khoản hoặc mật khẩu không để trống";
                return View("Index");
            }
            var md5pass = MD5Utils.MD5Hash(password);
            var acc = db.User.FirstOrDefault(x => x.UserName == userName && x.Password == md5pass);
            if(acc != null)
            {
                var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, userName)
                }, "BkapSecurityScheme");
                var pricipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync("BkapSecurityScheme", pricipal);
                return RedirectToAction("Index", "Dashboard");
            } else
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