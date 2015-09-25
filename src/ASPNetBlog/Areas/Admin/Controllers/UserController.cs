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
using Microsoft.AspNet.Identity;

// Ritesh Pahwa RiteshPahwa.com Asp.Net 5 MVC 6
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles="Admin,UserAdmin")]
    public class UserController : BaseController
    {
        [FromServices]
        public UserManager<ApplicationUser> UserManager {get; set;}


        protected override void ActionStartup() { ViewBag.EntityTitle = "User"; }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "List";
            return View();
        }

        public async Task<JsonResult> Paging()
        {
            var dtParam = await DataTables.BindModelAsync(Request.Query);

            var data = Db.Users.Select(d => new { d.Id, d.UserName, d.FullName, d.Email, d.AppUserId, d.Activated, d.EmailConfirmed, d.AccessFailedCount, d.LockoutEnabled, d.TwoFactorEnabled });
            if (data == null) return null;

            var filteredData = data.Where(w => w.FullName.Contains(dtParam.sSearch) || w.Email.Contains(dtParam.sSearch));
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }
        
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new UserViewModel();
            ViewBag.Title = "Add";
            await SetViewBagRoles();
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newModel = Mapper.Map(model, new ApplicationUser(), d => new {
                    d.Email,
                    d.EmailConfirmed,
                    d.FullName,
                    d.AccessFailedCount,
                    d.LockoutEnabled,
                    d.LockoutEnd,
                    d.PhoneNumber,
                    d.PhoneNumberConfirmed,
                    d.TwoFactorEnabled
                }); // Map only specific fields

                newModel.UserName = model.Email;

                var normalUser = await UserManager.FindByEmailAsync(model.Email);
                if (normalUser == null)
                {
                    await UserManager.CreateAsync(newModel, "blogP@$$w0rd");
                }

                foreach (var r in model.UserRoles) Db.UserRoles.Add(new IdentityUserRole<string>() { UserId = newModel.Id, RoleId = r });

                await Db.SaveChangesAsync();

                //TempData.Success("User was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("User was not saved! Please check errors.");

            ViewBag.Title = "Add";
            await SetViewBagRoles();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var result = await Db.Users.Include(d=> d.Roles).Where(d => d.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result, new UserViewModel(), true, d => new { d.UserRoles });
            model.UserRoles = result.Roles.Select(d => d.RoleId);

            ViewBag.Title = "Edit";
            await SetViewBagRoles();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Users.Include(d=>d.Roles).FirstAsync(d => d.Id == model.Id);
                Mapper.Map(model, dbModel, d => new {
                    d.Email, d.EmailConfirmed, d.FullName, d.AccessFailedCount, d.LockoutEnabled,
                    d.LockoutEnd, d.PhoneNumber, d.PhoneNumberConfirmed, d.TwoFactorEnabled }); // Map only specific fields

                dbModel.UserName = model.Email;

                // Need to fix ConcurrencyStamp check
                //Db.Entry(dbModel).OriginalValues["ConcurrencyStamp"] = model.ConcurrencyStamp;
                
                var dbUserRoles = dbModel.Roles.ToList();
                
                var addedRoles = model.UserRoles.Except(dbUserRoles.Select(s=> s.RoleId).DefaultIfEmpty());
                var deletedRoles = dbUserRoles.Where( r => !model.UserRoles.Any(t => t == r.RoleId)).ToList();

                foreach (var r in addedRoles) Db.UserRoles.Add(new IdentityUserRole<string>() { UserId = dbModel.Id, RoleId = r });
                foreach (var r in deletedRoles) Db.UserRoles.Remove(r);

                Db.Update(dbModel);
                // Bug autmatically marking Identity Col to Modified
                Db.Entry(dbModel).Property(p => p.AppUserId).IsModified = false;

                await Db.SaveChangesAsync();

                //TempData.Success("User was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("User was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            await SetViewBagRoles();
            return View("Add", model);
        }

        private async Task SetViewBagRoles()
        {
            ViewBag.Roles = await Db.Roles.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
        }

    }
}
