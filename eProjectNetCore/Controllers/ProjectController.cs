using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create([Bind("CompetitionId,Image,Description,Price")] Project project)
        {
            if (ModelState.IsValid)
            {
                var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
                if (userDto.id == null)
                {
                    return NotFound();
                }
                var files = HttpContext.Request.Form.Files;

                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\admin\\images\\project", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        project.Image = "images/project/" + FileName;
                    }
                }
                project.AccountId = userDto.id;
                project.CreatedDate = DateTime.Now;
                project.UpdatedDate = DateTime.Now;
                project.Status = "SUBMITTED";
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "");
            }
            return View(project);
        }

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
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string idUpdate, [Bind("Id,Image,Description,Price")] Project project)
        {
            if (idUpdate != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var projectDB = await _context.Project.FindAsync(project.Id);
                    if (projectDB == null)
                    {
                        return NotFound();
                    }
                    var userDto = JsonConvert.DeserializeObject<UserDto>(HttpContext.Session.GetString("SessionUser"));
                    if (userDto.id == null)
                    {
                        return NotFound();
                    }
                    var files = HttpContext.Request.Form.Files;

                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\admin\\images\\project", FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            project.Image = "images/project/" + FileName;
                        }
                    }
                    projectDB.UpdatedDate = DateTime.Now;
                    projectDB.Image = project.Image;
                    projectDB.Price = project.Price;
                    projectDB.Description = project.Description;
                    _context.Update(projectDB);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(project.Id))
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
            return View(project);
        }

        private bool CompetitionExists(object id)
        {
            throw new NotImplementedException();
        }
    }
}