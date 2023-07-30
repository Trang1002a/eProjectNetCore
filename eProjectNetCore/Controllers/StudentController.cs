using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eProjectNetCore.Controllers
{
    public class StudentController : Controller
    {
        AppDbContext _context;
        public StudentController(AppDbContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
            if (userDto.id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Class)
                .FirstOrDefaultAsync(m => m.Id == userDto.id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
    }
}