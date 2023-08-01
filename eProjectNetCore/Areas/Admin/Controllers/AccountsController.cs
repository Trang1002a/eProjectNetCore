using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eProjectNetCore.Data;
using eProjectNetCore.Models;
using eProjectNetCore.Utils;
using X.PagedList;
using System.IO;
using System.Security.Claims;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.Account.Where(a => a.Status == "ACTIVE").Include(a => a.Class).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.Account.Where(a => a.Status == "ACTIVE").Include(a => a.Class).Where(a => a.Name.Contains(name)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            List<Class> classes = _context.Class.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = classes;
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserName,Email,Phone,Address,Password,ClassId,Status,Birthday,CreatedDate,UpdatedDate,Avatar")] Account account)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Account
                .FirstOrDefaultAsync(m => m.UserName == account.UserName);
                if (check != null)
                {
                    TempData["StatusFailed"] = "Student with the same username already exists";
                    return RedirectToAction(nameof(Create));
                }
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\admin\\images\\avatar", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        account.Avatar = "images/avatar/" + FileName;
                    }
                }
                String passwordMD5 = MD5Utils.MD5Hash(account.Password);
                account.Password = passwordMD5;
                account.CreatedDate = DateTime.Now;
                account.UpdatedDate = DateTime.Now;
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Id", account.ClassId);
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            List<Class> classes = _context.Class.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = classes;
            ViewBag.menu = Load();
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email,Phone,Address,ClassId,Status,Birthday,Avatar")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var accountNew = await _context.Account.FindAsync(account.Id);
                    if (accountNew == null)
                    {
                        return NotFound();
                    }
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;
                        if(FileName != null)
                        {
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\admin\\images\\avatar", FileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                                accountNew.Avatar = "images/avatar/" + FileName;
                            }
                        }
                        
                    }

                    accountNew.Name = account.Name;
                    accountNew.Email = account.Email;
                    accountNew.Phone = account.Phone;
                    accountNew.Address = account.Address;
                    accountNew.ClassId = account.ClassId;
                    accountNew.Status = account.Status;
                    accountNew.Birthday = account.Birthday;
                    accountNew.UpdatedDate = DateTime.Now;
                    _context.Update(accountNew);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Id", account.ClassId);
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            account.Status = "DEACTIVE";
            _context.Update(account);
            await _context.SaveChangesAsync();
            TempData["StatusSuccess"] = "Successfully";
            return RedirectToAction(nameof(Index));
        }
        private bool AccountExists(string id)
        {
            ViewBag.menu = Load();
            return _context.Account.Any(e => e.Id == id);
        }

        public List<Menu> Load()
        {
            try
            {
                IEnumerable<Claim> claims = ((ClaimsIdentity)User.Identity).Claims;
                if (claims.Count() > 0)
                {
                    var acc = "";
                    foreach (var claim in claims)
                    {
                        acc = claim.Value;
                    }
                    var user = _context.User.FirstOrDefault(x => x.Id == acc);
                    if (user != null)
                    {
                        var package = _context.Package.FirstOrDefault(m => m.GroupId == user.GroupId);
                        if (package == null)
                        {
                            return new List<Menu>();
                        }
                        if (package != null)
                        {
                            List<Menu> menus = new List<Menu>();
                            string[] menuId = package.MenuId.Split(",");
                            List<string> lst = menuId.OfType<string>().ToList();
                            foreach (var x in lst)
                            {
                                var menu = _context.Menu.FirstOrDefault(m => m.Id == x);
                                menus.Add(menu);
                            }
                            return menus;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return new List<Menu>();
        }
    }
}
