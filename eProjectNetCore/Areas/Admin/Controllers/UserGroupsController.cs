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
    public class UserGroupsController : Controller
    {
        private readonly AppDbContext _context;

        public UserGroupsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserGroups
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.UserGroup.Where(a => a.Status == "ACTIVE").OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.UserGroup.Where(a => a.Status == "ACTIVE").Where(a => a.Name.Contains(name)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/UserGroups/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGroup == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Create
        public IActionResult Create()
        {
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/UserGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.UserGroup
                .FirstOrDefaultAsync(m => m.Name == userGroup.Name);
                if (check != null)
                {
                    TempData["StatusFailed"] = "UserGroup with the same name already exists";
                    return RedirectToAction(nameof(Create));
                }
                _context.Add(userGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.menu = Load();
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroup.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            ViewBag.menu = Load();
            return View(userGroup);
        }

        // POST: Admin/UserGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Name,Status")] UserGroup userGroup)
        {
            if (id != userGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGroupExists(userGroup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["StatusSuccess"] = "Successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.menu = Load();
            return View(userGroup);
        }

        // GET: Admin/UserGroups/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            try
            {
                var userGroup = await _context.UserGroup.FindAsync(id);
                _context.UserGroup.Remove(userGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch(Exception e)
            {
                TempData["StatusFailed"] = "Cannot delete , please check again";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/UserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var userGroup = await _context.UserGroup.FindAsync(id);
            _context.UserGroup.Remove(userGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserGroupExists(byte id)
        {
            return _context.UserGroup.Any(e => e.Id == id);
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
