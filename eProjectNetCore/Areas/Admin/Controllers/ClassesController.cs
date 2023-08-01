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
using System.Security.Claims;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassesController : Controller
    {
        private readonly AppDbContext _context;

        public ClassesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Classes
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.Class.Where(a => a.Status == "ACTIVE").OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.Class.Where(a => a.Status == "ACTIVE").Where(a => a.Name.Contains(name)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Classes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(@class);
        }

        // GET: Admin/Classes/Create
        public IActionResult Create()
        {
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreatedDate,UpdatedDate")] Class @class)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Account
                .FirstOrDefaultAsync(m => m.Name == @class.Name);
                if (check != null)
                {
                    TempData["StatusFailed"] = "Class with the same name already exists";
                    return RedirectToAction(nameof(Create));
                }
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.menu = Load();
            return View(@class);
        }

        // GET: Admin/Classes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(@class);
        }

        // POST: Admin/Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Status,CreatedDate,UpdatedDate")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            ViewBag.menu = Load();
            return View(@class);
        }

        // GET: Admin/Classes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }
            @class.Status = "DEACTIVE";
            _context.Update(@class);
            await _context.SaveChangesAsync();
            TempData["StatusSuccess"] = "Successfully";
            return RedirectToAction(nameof(Index));
        }
       

        // POST: Admin/Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @class = await _context.Class.FindAsync(id);
            _context.Class.Remove(@class);
            await _context.SaveChangesAsync();
            ViewBag.menu = Load();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(string id)
        {
            return _context.Class.Any(e => e.Id == id);
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
