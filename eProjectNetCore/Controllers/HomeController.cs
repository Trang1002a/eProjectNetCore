using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eProjectNetCore.Models;
using eProjectNetCore.Data;
using Microsoft.EntityFrameworkCore;

namespace eProjectNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Competition> competitions = _context.Competition.Where(com => com.Status == "ACTIVE").ToList();
            ViewBag.data = competitions;
            return View();
        }

        public IActionResult Teachears()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Teachers()
        {
            List<User> users = _context.User
                .Where(com => com.Status == "ACTIVE")
                .Where(com => com.GroupId == 2)
                .Take(3).ToList();
            ViewBag.data = users;
            return View();
        }

        public IActionResult Exhibition()
        {
            var accounts = _context.Project
                .Include(p => p.Account)
                .Include(p => p.Competition)
                .Include(p => p.User)
                .OrderBy(a => a.Id)
                .Where(a => a.Status == "EXHIBITION")
                .Take(3)
                .ToList();
            ViewBag.data = accounts;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
