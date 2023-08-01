using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Models;
using eProjectNetCore.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                ViewBag.error = "Account or password is not blank";
                return View("Index");
            }
            var md5pass = MD5Utils.MD5Hash(password);
            var acc = db.User.FirstOrDefault(x => x.UserName == userName && x.Password == md5pass);
            if(acc != null)
            {
                HttpContext.Session.SetString("AdminId", acc.Id);
                var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Sid, acc.Id),
                }, "AdminSecurityScheme");
                var pricipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync("AdminSecurityScheme", pricipal);
                return RedirectToAction("Index", "Dashboard");
            } else
            {
                ViewBag.error = "Invalid account or password";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("AdminSecurityScheme");
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Index", "Login");
        }
    }
}