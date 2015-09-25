using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ASPNetBlog.Models;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Mvc.Rendering;
using ASPNetBlog.App_Common.Session;
using ASPNetBlog.App_Common.Extensions;
using ASPNetBlog.App_Common.Base;
using Microsoft.AspNet.Authorization;
using ASPNetBlog.App_Common.Mapper;
using ASPNetBlog.App_Common.Paging;
using System.Collections.Generic;

// Ritesh Pahwa RiteshPahwa.com @ 9-12-15
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    [Authorize(Roles="Admin,PostAdmin")] //Should Implement Claims here
    public class PostController : BaseController
    {
        protected override void ActionStartup() { ViewBag.EntityTitle = "Post"; }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "List";
            return View();
        }

        public async Task<JsonResult> Paging()
        {
            var dtParam = await DataTables.BindModelAsync(Request.Query);

            var data = Db.Posts.OrderByDescending(p => p.PostedOn).Select(d => 
                new { d.Id, d.Title, d.Slug, PostedOn = d.PostedOn.ToString() });
            //Display Categories if needed Categories = (from c in d.Categories select c.Category.Name into c2 select c2.DefaultIfEmpty()).Aggregate((c, cn) => c + "," + cn)

            if (data == null) return null;

            var filteredData = data.Where(w => w.Title.Contains(dtParam.sSearch));
            var dataPage = filteredData.Skip(dtParam.iDisplayStart).Take(dtParam.iDisplayLength);
            var response = new { sEcho = dtParam.sEcho, iTotalRecords = data.Count(), iTotalDisplayRecords = filteredData.Count(), aaData = dataPage };

            return new JsonResult(response); // ( (JsonResult)new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new PostViewModel();
            ViewBag.Title = "Add";
            model.PostedOn = DateTime.Now;
            model.NewCommentAllowed = true;
            await SetViewItemsAsync();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Slug = model.Slug.ToSlug(); // Remove any special chars etc.
                var newModel = Mapper.Map(model, new Post(), m => new { m.Title, m.Slug, m.ShortDescription, m.Content, m.PostedOn, m.PublishStatus, m.Visibility, m.AuthorId });

                newModel.CreatedOn = DateTime.Now;
                newModel.CreatedBy = AppSession.AppUserId;
                newModel.CommentAllowed = model.NewCommentAllowed;
                
                Db.Posts.Add(newModel);
                await Db.SaveChangesAsync();

                foreach (var pc in model.PostCategories) newModel.Categories.Add(new PostCategory() { PostId = newModel.Id, CategoryId = pc});
                model.PostTags = await AddNewTagsAsync(model.PostTags);
                foreach (var pt in model.PostTags) newModel.Tags.Add(new PostTag() { PostId = newModel.Id, TagId = int.Parse(pt) });
                await Db.SaveChangesAsync();
                
                return RedirectToAction("Index");
            }
            //TempData.Danger("Post was not saved! Please check errors.");

            ViewBag.Title = "Add";
            await SetViewItemsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var query = from c in Db.Posts.Where(d => d.Id == id).Include(d => d.Categories).Include(d => d.Tags)
                        from au in Db.Users.Where(u => u.AppUserId == c.CreatedBy).Select(u => u.FullName).DefaultIfEmpty()
                        from mu in Db.Users.Where(u => u.AppUserId == c.ModifiedBy).Select(u => u.FullName).DefaultIfEmpty()
                        select new { Post = c, AddUser = au, ModUser = mu };

            var result = await query.SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result.Post, new PostViewModel(), exclude: m => new { m.CommentAllowed });
            model.NewCommentAllowed = result.Post.CommentAllowed ?? false;
            model.PostCategories = result.Post.Categories.Select(c => c.CategoryId).DefaultIfEmpty();
            model.PostTags = result.Post.Tags.Select(t => t.TagId.ToString()).DefaultIfEmpty().ToList();

            model.CreatedByName = result.AddUser;
            model.ModifiedByName = result.ModUser;

            ViewBag.Title = "Edit";
            await SetViewItemsAsync();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Posts.Include(d => d.Categories).Include(d => d.Tags).FirstAsync(c => c.Id == model.Id);
                model.Slug = model.Slug.ToSlug(); // Remove any special chars etc.
                Mapper.Map(model, dbModel, m => new { m.Title, m.Slug, m.ShortDescription, m.Content, m.PostedOn, m.PublishStatus, m.Visibility, m.AuthorId });
                
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.ModifiedBy = AppSession.AppUserId;
                dbModel.CommentAllowed = model.NewCommentAllowed;

                foreach (var c in dbModel.Categories.Where(c => !model.PostCategories.Any(t => t == c.CategoryId)).ToList())
                    Db.PostCategories.Remove(c);
                foreach (var c in model.PostCategories.Except(dbModel.Categories.Select(s => s.CategoryId).DefaultIfEmpty()))
                    dbModel.Categories.Add(new PostCategory() { PostId = dbModel.Id, CategoryId = c});

                model.PostTags = await AddNewTagsAsync(model.PostTags);
                foreach (var t in dbModel.Tags.Where(c => !model.PostTags.Any(t => t == c.TagId.ToString())).ToList())
                    Db.PostTags.Remove(t);
                foreach (var t in model.PostTags.Except(dbModel.Tags.Select(s => s.TagId.ToString()).DefaultIfEmpty()))
                    dbModel.Tags.Add(new PostTag() { PostId = dbModel.Id, TagId = int.Parse(t) });

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Post was successfully saved!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Post was not saved! Please check errors.");

            ViewBag.Title = "Edit";
            await SetViewItemsAsync();
            return View("Add", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Db.Posts.Where(c => c.Id == id).SingleOrDefaultAsync();
            if (result == null) return HttpNotFound();

            var model = Mapper.Map(result, new DeleteViewModel(), m => new { m.Id, m.RowStamp });
            model.ItemInfo = result.Title;

            ViewBag.Title = "Delete";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Can we just update DB without fetching first, need ideas, also concurrency check needs to be done
                var dbModel = await Db.Posts.SingleOrDefaultAsync(d => d.Id == model.Id);
                if (dbModel == null) return HttpNotFound();

                dbModel.PublishStatus = PublishStatus.Deleted;

                // Need to fix RowStamp check
                //Db.Entry(dbModel).OriginalValues["RowStamp"] = model.RowStamp;
                Db.Update(dbModel);
                await Db.SaveChangesAsync();

                //TempData.Success("Post was successfully marked as deleted!");
                return RedirectToAction("Index");
            }
            //TempData.Danger("Post was not deleted! Please check errors.");

            ViewBag.Title = "Delete";
            return View(model);
        }

        private async Task SetViewItemsAsync()
        {
            ViewBag.Authors = await Db.Authors.OrderBy(c=>c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
            ViewBag.Categories = await Db.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
            ViewBag.Tags = await Db.Tags.OrderBy(c => c.Name).Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
            ViewBag.PublishStatuses = new PublishStatus().SelectListItems();
            ViewBag.Visibility = new Visibility().SelectListItems();
        }

        private async Task<IList<string>> AddNewTagsAsync(IList<string> tags)
        {
            //coping list so that we can add/remove items
            var newTags = new List<string>(tags);
            foreach(var item in newTags.Where(t=> t.StartsWith("||")).ToList())
            {
                var tag = new Tag() { Name = item.Substring(2) };
                Db.Tags.Add(tag);
                await Db.SaveChangesAsync();
                newTags.Remove(item);
                newTags.Add(tag.Id.ToString());
            }
            return newTags;
        }
    }
}
