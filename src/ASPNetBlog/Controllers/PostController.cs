using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ASPNetBlog.App_Common.Base;
using Microsoft.Data.Entity;
using ASPNetBlog.App_Common.Mapper;
using ASPNetBlog.Models;
using ASPNetBlog.App_Common.Session;
using ASPNetBlog.Business;

// Ritesh Pahwa RiteshPahwa.com @ 9-19-15
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Controllers
{
    public class PostController : BaseController
    {
        // GET: /<controller>/
        [Route("/Post/{id:int}/{slug}")]
        public async Task<IActionResult> Index(int id = 0, string slug = "")
        {
            if (id == 0 || string.IsNullOrEmpty(slug))
            {
                return Redirect("/");
            }

            var data = new PostData(Db);
            var model = await data.GetByIdAsync(id);
            if (model == null) Redirect("/");            

            return View(model);
        }

        [Route("/Category/{id:int}/{slug}")]
        public async Task<IActionResult> Category(int id = 0, string slug = "", int p = 1)
        {
            if (id == 0 || string.IsNullOrEmpty(slug))
            {
                return Redirect("/");
            }
            if (p <= 0) p = 1;

            var data = new PostData(Db);
            var model = await data.GetByCategoryAsync(id, p);
            if (model == null) Redirect("/");
            ViewBag.PageCount = data.PageCount;
            ViewBag.PageNo = p;
            ViewBag.PagingUrl = $"/Category/{id}/{slug}?p={{p}}";

            return View("/Views/Home/Index", model);
        }

        [Route("/Tag/{id:int}/{slug}")]
        public async Task<IActionResult> Tag(int id = 0, string slug = "", int p = 1)
        {
            if (id == 0 || string.IsNullOrEmpty(slug))
            {
                return Redirect("/");
            }
            if (p <= 0) p = 1;

            var data = new PostData(Db);
            var model = await data.GetByTagAsync(id, p);
            if (model == null) Redirect("/");
            ViewBag.PageCount = data.PageCount;
            ViewBag.PageNo = p;
            ViewBag.PagingUrl = $"/Tag/{id}/{slug}?p={{p}}";

            return View("/Views/Home/Index", model);
        }

        [Route("/Post/AddComment/{postId:int}")]
        public async Task<IActionResult> AddComment(int postId = 0, PostCommentViewModel model = null)
        {
            if(ModelState.IsValid)
            {
                var newModel = Mapper.Map(model, new PostComment(), x=> new { x.Name, x.Email, x.Comment, x.PostId, x.ParentCommentId });
                newModel.CommentStatus = CommentStatus.WaitingForReview;
                newModel.CreatedOn = DateTime.Now;
                newModel.CreatedBy = AppSession.AppUserId;

                Db.PostComments.Add(newModel);
                await Db.SaveChangesAsync();
            } else {
                return Content(string.Join(",", ModelState.Keys.SelectMany(x=> ModelState[x].Errors).DefaultIfEmpty()));
            }

            return Content("Looks Good!");
        }
    }
}
