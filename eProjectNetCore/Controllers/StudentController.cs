using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eProjectNetCore.Data;
using eProjectNetCore.Dto;
using eProjectNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

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

        public async Task<IActionResult> ResetPassword()
        {
            var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
            if (userDto.id == null)
            {
                return NotFound();
            }
            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == userDto.id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        public async Task<IActionResult> ExamSubmitted(int page = 1)
        {
            var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
            if (userDto.id == null)
            {
                return NotFound();
            }
            int limit = 10;
            var account = await _context.Project
                .Include(p => p.Account)
                .Include(p => p.Competition)
                .Include(p => p.User)
                .Where(p => p.AccountId == userDto.id)
                .OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            return View(account);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Account)
                .Include(p => p.Competition)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(String oldPassword, String password, String password2)
        {
            var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
            if (userDto.id == null)
            {
                return NotFound();
            }
            if (String.IsNullOrEmpty(oldPassword) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password2))
            {
                    ViewBag.error = "Password can not be blank";
                    return View("ResetPassword");
            }

            if (password != password2)
            {
                ViewBag.error = "New password is not the same";
                return View("ResetPassword");
            }

            var account = await _context.Account
                   .FirstOrDefaultAsync(m => m.Id == userDto.id);
            if (account == null)
            {
                return NotFound();
            }
            var md5pass = MD5Utils.MD5Hash(oldPassword);
            if(md5pass != account.Password) {
                ViewBag.error = "Old password is not correct";
                return View("ResetPassword");
            }
            var md5passNew = MD5Utils.MD5Hash(password);
            account.Password = md5passNew;
            account.UpdatedDate = DateTime.Now;
            _context.Update(account);
            await _context.SaveChangesAsync();
            ViewBag.success = "Change password successfully";
            return View("ResetPassword");
        }
    }
}