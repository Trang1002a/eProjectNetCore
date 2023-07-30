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
        public async Task<IActionResult> Index(String name, int page = 1)
        {
            int limit = 10;
            var account = await _context.Project.Include(p => p.Account).Include(p => p.Competition).Include(p => p.User).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            if (!String.IsNullOrEmpty(name))
            {
                account = await _context.Project.Where(a => a.CompetitionId.Contains(name)).OrderBy(a => a.Id).ToPagedListAsync(page, limit);
            }
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

            return View(project);
        }

        // GET: Admin/Projects/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "Id");
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Id");
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
            return View(project);
        }

        // GET: Admin/Projects/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Account, "Id", "Id", project.AccountId);
            ViewData["CompetitionId"] = new SelectList(_context.Competition, "Id", "Id", project.CompetitionId);
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
            return View(project);
        }
        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var project = await _context.Project.FindAsync(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(string id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
