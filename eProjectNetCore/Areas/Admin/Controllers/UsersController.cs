using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eProjectNetCore.Data;
using eProjectNetCore.Models;
using X.PagedList;
using eProjectNetCore.Utils;
using System.Security.Claims;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.User.Where(a => a.Status == "ACTIVE").Include(a => a.Group).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.User.Where(a => a.Status == "ACTIVE").Include(a => a.Group).Where(a => a.Name.Contains(name)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            List<UserGroup> userGroups = _context.UserGroup.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = userGroups;
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,UserName,Phone,Address,Password,Status,GroupId,CreatedDate,UpdatedDate")] User user)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.User
                .FirstOrDefaultAsync(m => m.UserName == user.UserName);
                if (check != null)
                {
                    TempData["StatusFailed"] = "User with the same username already exists";
                    return RedirectToAction(nameof(Create));
                }
                String passwordMD5 = MD5Utils.MD5Hash(user.Password);
                user.Password = passwordMD5;
                user.CreatedDate = DateTime.Now;
                user.UpdatedDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.UserGroup, "Id", "Id", user.GroupId);
            ViewBag.menu = Load();
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            List<UserGroup> userGroups = _context.UserGroup.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = userGroups;
            ViewBag.menu = Load();
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email,UserName,Phone,Address,Status,GroupId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userNew = await _context.User.FindAsync(user.Id);
                    if (userNew == null)
                    {
                        return NotFound();
                    }
                    userNew.Name = user.Name;
                    userNew.Email = user.Email;
                    userNew.Phone = user.Phone;
                    userNew.Address = user.Address;
                    userNew.GroupId = user.GroupId;
                    userNew.Status = user.Status;
                    userNew.CreatedDate = DateTime.Now;
                    userNew.UpdatedDate = DateTime.Now;
                    _context.Update(userNew);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["GroupId"] = new SelectList(_context.UserGroup, "Id", "Id", user.GroupId);
            ViewBag.menu = Load();
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.User
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

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.Id == id);
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
