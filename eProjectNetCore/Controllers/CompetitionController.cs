using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Dto;
using eProjectNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eProjectNetCore.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly AppDbContext _context;

        public CompetitionController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DateTime timeNow = DateTime.Now;
            List<Competition> competitions = _context.Competition
                .Where(com => com.Status == "ACTIVE")
                .Where(com => com.EndDate > timeNow)
                .ToList();
            ViewBag.data = competitions;
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competition = await _context.Competition
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            try {
                var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
                var account = await _context.Account
                    .Include(a => a.Class)
                    .FirstOrDefaultAsync(m => m.Id == userDto.id);
                ViewBag.user = account;
                var project = await _context.Project
                    .FirstOrDefaultAsync(m => m.AccountId == userDto.id && m.CompetitionId == id);
            } catch (Exception e)
            {
                ViewBag.user = null;
            }
            ViewBag.submit = false;
            if (competition.EndDate > DateTime.Now)
            {
                ViewBag.submit = true;
            }
            return View(competition);
        }
    }
}