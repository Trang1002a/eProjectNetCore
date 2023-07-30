using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Dto;
using eProjectNetCore.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                ViewBag.error = "Incorrect account or password";
                return View("Index");
            }
            var md5pass = MD5Utils.MD5Hash(password);
            var acc = _context.Account.FirstOrDefault(x => x.UserName == userName && x.Password == md5pass);
            if (acc != null)
            {
                UserDto userDto = new UserDto();
                userDto.id = acc.Id;
                userDto.userName = acc.UserName;
                HttpContext.Session.SetString("SessionUser", JsonConvert.SerializeObject(userDto));
                HttpContext.Session.SetString("NameUser", userName);
                return RedirectToAction("Index", "");
            }
            else
            {
                ViewBag.error = "Incorrect account or password";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("SessionUser");
            HttpContext.Session.Remove("NameUser");
            return RedirectToAction("Index", "Login");
        }
    }
}