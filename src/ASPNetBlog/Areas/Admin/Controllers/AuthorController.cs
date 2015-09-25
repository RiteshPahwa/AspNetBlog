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

// Ritesh Pahwa RiteshPahwa.com @ 9-12-15
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    [Authorize(Roles="Admin,AuthorAdmin")] //Should Implement Claims here
    public class AuthorController : BaseController
    {
        protected override void ActionStartup() { ViewBag.EntityTitle = "Author"; }

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
            //var data = Db.Authors.Select(d => new { d.Id, d.Name, d.Slug, ParentAuthorName = d.ParentAuthor.Name }); 

            var data = Db.Authors.Select(d => new{ d.Id, d.Name, d.Slug, d.AppUserId });

            if (data == null) return null;

            var filteredData = data.Where(w => w.Name.Contains(dtParam.sSearch));
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AuthorViewModel();
            ViewBag.Title = "Add";
            await SetViewBagAuthors();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newModel = Mapper.Map(model, new Author(), m => new { m.Name, m.Slug, m.AppUserId, m.ShortDescription, m.Content });

                newModel.CreatedOn = DateTime.Now;
                newModel.CreatedBy = AppSession.AppUserId;

                Db.Authors.Add(newModel);
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //TempData.Danger("Author was not saved! Please check errors.");

            ViewBag.Title = "Add";
            await SetViewBagAuthors();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var query = from c in Db.Authors.Where(c => c.Id == id)
                        from au in Db.Users.Where(u => u.AppUserId == c.CreatedBy).Select(u => u.FullName).DefaultIfEmpty()
                        from mu in Db.Users.Where(u => u.AppUserId == c.ModifiedBy).Select(u => u.FullName).DefaultIfEmpty()
                        select new { Cat = c, AddUser = au, ModUser = mu };

            var result = await query.SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result.Cat, new AuthorViewModel());

            model.CreatedByName = result.AddUser;
            model.ModifiedByName = result.ModUser;

            ViewBag.Title = "Edit";
            await SetViewBagAuthors();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Authors.FirstAsync(c => c.Id == model.Id);
                Mapper.Map(model, dbModel, m => new { m.Name, m.Slug, m.AppUserId, m.ShortDescription, m.Content });

                dbModel.ModifiedOn = DateTime.Now;
                dbModel.ModifiedBy = AppSession.AppUserId;

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Author was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Author was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            await SetViewBagAuthors();
            return View("Add", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Db.Authors.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result, new AuthorViewModel());

            ViewBag.Title = "Delete";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Authors.SingleOrDefaultAsync(d => d.Id == model.Id);
                if (dbModel == null) return HttpNotFound();

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Authors.Remove(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Author was successfully deleted!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Author was not deleted! Please check errors.");

            ViewBag.Title = "Delete";
            return View(model);
        }

        private async Task SetViewBagAuthors()
        {
            ViewBag.Authors = await Db.Users.Select(c => new SelectListItem() { Text = $"{c.FullName} <{c.UserName}>" , Value = c.AppUserId.ToString() }).ToListAsync();
        }

    }
}
