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
using ASPNetBlog.App_Common.Extensions;

// Ritesh Pahwa RiteshPahwa.com @ 9-12-15
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    //[Authorize(Roles="Admin,CategoryAdmin")] //Should Implement Claims here
    public class CategoryController : BaseController
    {
        protected override void ActionStartup() { ViewBag.EntityTitle = "Category"; }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "List";
            return View();
        }

        public async Task<JsonResult> Paging()
        {
            var dtParam = await DataTables.BindModelAsync(Request.Query);

            // This one works in LinqPad so could be bug in EF Beta
            //var data = Db.Categories.Select(d => new { d.Id, d.Name, d.Slug, ParentCategoryName = d.ParentCategory.Name }); 

            var data = from d in Db.Categories
                       select new
                       {
                           d.Id, d.Name, d.Slug,
                           ParentCategoryName = 
                            (from p in Db.Categories where p.Id == d.ParentCategoryId select p.Name).FirstOrDefault()
                       };

            if (data == null) return null;

            var filteredData = data.Where(w => w.Name.Contains(dtParam.sSearch)).OrderByDescending(c => c.Id);
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new CategoryViewModel();
            ViewBag.Title = "Add";
            await SetViewBagCategories();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Slug = model.Slug.ToSlug(); // Remove any special chars etc.
                var newModel = Mapper.Map(model, new Category(), m => new { m.Name, m.Slug, m.ParentCategoryId });

                newModel.CreatedOn = DateTime.Now;
                newModel.CreatedBy = AppSession.AppUserId;

                Db.Categories.Add(newModel);
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //TempData.Danger("Category was not saved! Please check errors.");

            ViewBag.Title = "Add";
            await SetViewBagCategories();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var query = from c in Db.Categories.Where(c => c.Id == id)
                        from au in Db.Users.Where(u => u.AppUserId == c.CreatedBy).Select(u => u.FullName).DefaultIfEmpty()
                        from mu in Db.Users.Where(u => u.AppUserId == c.ModifiedBy).Select(u => u.FullName).DefaultIfEmpty()
                        select new { Cat = c, AddUser = au, ModUser = mu };

            var result = await query.SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result.Cat, new CategoryViewModel());

            model.CreatedByName = result.AddUser;
            model.ModifiedByName = result.ModUser;

            ViewBag.Title = "Edit";
            await SetViewBagCategories();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Categories.FirstAsync(c => c.Id == model.Id);
                model.Slug = model.Slug.ToSlug(); // Remove any special chars etc.
                Mapper.Map(model, dbModel, c => new { c.Name, c.Slug, c.ParentCategoryId });

                dbModel.ModifiedOn = DateTime.Now;
                dbModel.ModifiedBy = AppSession.AppUserId;

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Category was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Category was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            await SetViewBagCategories();
            return View("Add", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Db.Categories.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result, new CategoryViewModel());

            ViewBag.Title = "Delete";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Categories.SingleOrDefaultAsync(d => d.Id == model.Id);
                if (dbModel == null) return HttpNotFound();

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Categories.Remove(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Category was successfully deleted!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Category was not deleted! Please check errors.");

            ViewBag.Title = "Delete";
            return View(model);
        }

        private async Task SetViewBagCategories()
        {
            ViewBag.Categories = await Db.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
        }

    }
}
