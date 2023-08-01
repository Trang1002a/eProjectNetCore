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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace eProjectNetCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Projects
        public async Task<IActionResult> Index(String status, int page = 1)
        {
            int limit = 10;
            var account = await _context.Project.Include(p => p.Account).Include(p => p.Competition).Include(p => p.User).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(status))
            {
                account = await _context.Project.Include(p => p.Account).Include(p => p.Competition).Include(p => p.User)
                    .Where(a => a.Status.Contains(status)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
            ViewBag.menu = Load();
            return View(account);
        }

        // GET: Admin/Projects/Details/5
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
            ViewBag.menu = Load();
            return View(project);
        }

        // GET: Admin/Projects/Create
        public IActionResult Create()
        {
            List<Account> accounts = _context.Account.Where(a => a.Status == "ACTIVE").ToList();
            List<Competition> competitions = _context.Competition.Where(a => a.Status == "ACTIVE").ToList();
            List<string> listStatus = new List<string>(new string[] { "SUBMITTED", "EVALUATED", "EXHIBITION" , "SOLD"});
            ViewBag.listStatus = listStatus;
            ViewBag.accounts = accounts;
            ViewBag.competitions = competitions;
            ViewBag.menu = Load();
            return View();
        }

        // POST: Admin/Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Description,AccountId,Price,CreatedDate,UpdatedDate,Status,CompetitionId")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "Id", project.AccountId);
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Id", project.CompetitionId);
            ViewBag.menu = Load();
            return View(project);
        }

        // GET: Admin/Projects/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            List<Account> accounts = _context.Account.Where(a => a.Status == "ACTIVE").ToList();
            List<Competition> competitions = _context.Competition.Where(a => a.Status == "ACTIVE").ToList();
            List<string> listStatus = new List<string>(new string[] { "SUBMITTED", "EVALUATED", "EXHIBITION", "SOLD" });
            ViewBag.listStatus = listStatus;
            ViewBag.accounts = accounts;
            ViewBag.competitions = competitions;
            ViewBag.menu = Load();
            return View(project);
        }

        // POST: Admin/Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Image,Description,AccountId,Price,CreatedDate,UpdatedDate,Status,CompetitionId")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "Id", project.AccountId);
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Id", project.CompetitionId);
            ViewBag.menu = Load();
            return View(project);
        }

        public async Task<IActionResult> Evaluate(string id)
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
            ViewBag.menu = Load();
            return View(project);
        }
        [HttpPost]
        public async Task<IActionResult> Evaluate(string id, [Bind("Id,Mark,Comment")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = HttpContext.Session.GetString("AdminId");
                    var projectDb = await _context.Project
                        .Include(p => p.Account)
                        .Include(p => p.Competition)
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (project == null)
                    {
                        return NotFound();
                    }
                    projectDb.Mark = project.Mark;
                    projectDb.Comment = project.Comment;
                    projectDb.Status = "EVALUATED";
                    projectDb.UserId = userId;
                    projectDb.UpdatedDate = DateTime.Now;
                    _context.Update(projectDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "Id", project.AccountId);
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Id", project.CompetitionId);
            ViewBag.menu = Load();
            return View(project);
        }
        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Project
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

        private bool ProjectExists(string id)
        {
            return _context.Project.Any(e => e.Id == id);
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
