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
    //[Authorize(Roles="Admin,RoleAdmin")]
    public class RoleController : BaseController
    {
        protected override void ActionStartup() { ViewBag.EntityTitle = "Role"; }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "List";
            return View();
        }

        public async Task<JsonResult> Paging()
        {
            var dtParam = await DataTables.BindModelAsync(Request.Query);

            var data = Db.Roles.Select(d=> new {d.Id, d.Name});
            if (data == null) return null;

            var filteredData = data.Where(w => w.Name.Contains(dtParam.sSearch));
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var result = await Db.Roles.Where(d => d.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = new RoleViewModel(result);

            ViewBag.Title = "Edit";
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Roles.FirstAsync(d => d.Id == model.Id);
                Mapper.Map(model, dbModel, d => new { d.Name}); // Map only specific fields

                // Need to fix ConcurrencyStamp check
                //Db.Entry(dbModel).OriginalValues["ConcurrencyStamp"] = model.ConcurrencyStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Role was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Role was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            return View("Add", model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new RoleViewModel();
            ViewBag.Title = "Add";
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newModel = Mapper.Map(model, new IdentityRole(), m => new { m.Name });

                Db.Roles.Add(newModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Role was successfully saved!");
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Add";
            return View(model);
        }

    }
}
