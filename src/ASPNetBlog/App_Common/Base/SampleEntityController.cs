using ASPNetBlog.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.App_Common.Base
{
    [Authorize]
    public abstract class SampleAbstractedEntityController<T, Id> : Controller where T : class, new()
    {
        [FromServices]
        public ApplicationDbContext Db { get; set; }

        public SampleAbstractedEntityController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await GetList().ToListAsync();
            if (model == null) return HttpNotFound();

            ViewBag.Title = "List";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Id id)
        {
            var model = await GetEdit(id).SingleOrDefaultAsync();
            if (model == null) return HttpNotFound();

            ViewBag.Title = "Edit";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(T model)
        {
            if (ModelState.IsValid)
            {
                Db.Users.Where(u => u.UserName == User.Identity.Name).Select(u=>u.AppUserId).SingleOrDefault();

                PostEdit(model);
                Db.Update(model);
                await Db.SaveChangesAsync();
            }

            ViewBag.Title = "Edit";
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = GetAdd(new T());
            ViewBag.Title = "Add";
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(T model)
        {
            if (ModelState.IsValid)
            {
                PostAdd(model);
                await Db.SaveChangesAsync();
            }

            ViewBag.Title = "Add";
            return View(model);
        }

        protected abstract IQueryable<T> GetList();
        protected abstract T GetAdd(T model);
        protected abstract T PostAdd(T model);
        protected abstract IQueryable<T> GetEdit(Id id);
        protected abstract T PostEdit(T model);
    }
}
