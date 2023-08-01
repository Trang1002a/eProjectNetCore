using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eProjectNetCore.Data;
using eProjectNetCore.Models;
using System.Security.Claims;
using X.PagedList;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PackagesController : Controller
    {
        private readonly AppDbContext _context;

        public PackagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Packages
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.Package.Where(a => a.Status == "ACTIVE").Include(a => a.Group).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.Package.Where(a => a.Name.Contains(name)).Where(a => a.Status == "ACTIVE").Include(a => a.Group).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Packages/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Package
                .Include(a => a.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (package == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(package);
        }

        // GET: Admin/Packages/Create
        public IActionResult Create()
        {
            List<UserGroup> classes = _context.UserGroup.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = classes;
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,GroupId,MenuId,CreatedDate,UpdatedDate")] Package package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.menu = Load();
            return View(package);
        }

        // GET: Admin/Packages/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Package.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            List<UserGroup> userGroups = _context.UserGroup.Where(a => a.Status == "ACTIVE").ToList();
            ViewBag.data = userGroups;
            ViewBag.menu = Load();
            return View(package);
        }

        // POST: Admin/Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Name,Status,GroupId,MenuId,CreatedDate,UpdatedDate")] Package package)
        {
            if (id != package.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.menu = Load();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.menu = Load();
            return View(package);
        }

        // GET: Admin/Packages/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Package
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            item.Status = "DEACTIVE";
            _context.Update(item);
            await _context.SaveChangesAsync();
            TempData["StatusSuccess"] = "Successfully";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var package = await _context.Package.FindAsync(id);
            _context.Package.Remove(package);
            await _context.SaveChangesAsync();
            ViewBag.menu = Load();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(byte id)
        {
            ViewBag.menu = Load();
            return _context.Package.Any(e => e.Id == id);
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
