using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ASPNetBlog.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Mvc.Rendering;
using ASPNetBlog.App_Common.Session;
using ASPNetBlog.App_Common.Base;
using Microsoft.AspNet.Authorization;
using ASPNetBlog.App_Common.Mapper;
using ASPNetBlog.App_Common.Paging;
using Microsoft.AspNet.Identity.EntityFramework;

// Ritesh Pahwa RiteshPahwa.com
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    //[Authorize(Tags="Admin,TagAdmin")] // Should be change to claims later
    public class TagController : BaseController
    {
        protected override void ActionStartup() { ViewBag.EntityTitle = "Tag";}

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "List";
            return View();
        }

        public async Task<JsonResult> Paging()
        {
            var dtParam = await DataTables.BindModelAsync(Request.Query);

            var data = Db.Tags.Select(d=> new {d.Id, d.Name});
            if (data == null) return null;

            var filteredData = data.Where(w => w.Name.Contains(dtParam.sSearch)).OrderByDescending(t => t.Id);
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var query = from t in Db.Tags.Where(s => s.Id == id)
                        from au in Db.Users.Where(u => u.AppUserId == t.CreatedBy).Select(u => u.FullName).DefaultIfEmpty()
                        from mu in Db.Users.Where(u => u.AppUserId == t.ModifiedBy).Select(u => u.FullName).DefaultIfEmpty()
                        select new { Tag = t, AddUser = au, ModUser = mu };

            var result = await query.SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result.Tag, new TagViewModel());

            model.CreatedByName = result.AddUser;
            model.ModifiedByName = result.ModUser;

            ViewBag.Title = "Edit";
            return View("Add", model);
        }

        //ToDo: Make Category and Tag inline editable in DataTables
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Tags.FirstAsync(d => d.Id == model.Id);
                Mapper.Map(model, dbModel, d => new { d.Name}); // Map only specific fields

                dbModel.ModifiedOn = DateTime.Now;
                dbModel.ModifiedBy = AppSession.AppUserId;

                // Need to fix ConcurrencyStamp check
                //Db.Entry(dbModel).OriginalValues["ConcurrencyStamp"] = model.ConcurrencyStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Tag was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Tag was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            return View("Add", model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new TagViewModel();
            ViewBag.Title = "Add";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newModel = Mapper.Map(model, new Tag(), m => new { m.Name });

                newModel.CreatedOn = DateTime.Now;
                newModel.CreatedBy = AppSession.AppUserId;

                Db.Tags.Add(newModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Tag was successfully saved!");
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Add";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Db.Tags.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result, new TagViewModel());

            ViewBag.Title = "Delete";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Tags.SingleOrDefaultAsync(c => c.Id == model.Id);
                if (dbModel == null) return HttpNotFound();

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Tags.Remove(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Tag was successfully deleted!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Tag was not deleted! Please check errors.");

            ViewBag.Title = "Delete";
            return View(model);
        }



    }
}
