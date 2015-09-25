using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ASPNetBlog.App_Common.Base;
using Microsoft.Data.Entity;
using ASPNetBlog.Business;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/22/15*/

namespace ASPNetBlog.Controllers
{
    public class HomeController : BaseController
    {
        //[Route("/Posts/{pageNo:int}")]
        public async Task<IActionResult> Index(int pageNo = 1)
        {        
            return await Latest(pageNo);
        }

        [Route("/Latest/{p?}")]
        public async Task<IActionResult> Latest(int p = 1)
        {
            if (p <= 0) p = 1;
            var data = new PostData(Db);
            var model = await data.LatestAsync(p);
            if (model == null) Redirect("/");
            ViewBag.PageCount = data.PageCount;
            ViewBag.PageNo = p;
            ViewBag.PagingUrl = "/Latest/{p}";
            return View("Index",model);
        }
        [Route("/Search")]
        public async Task<IActionResult> Search(string q = "", int p = 1)
        {
            if (string.IsNullOrWhiteSpace(q)) Redirect("/");
            var data = new PostData(Db);
            var model = await data.SearchAsync(q, p);
            if (model == null) Redirect("/");
            ViewBag.PageCount = data.PageCount;
            ViewBag.PageNo = p;
            ViewBag.PagingUrl = $"/Search?q={q}&p={{p}}";

            return View("Index", model);
        }

        public async Task<IActionResult> About()
        {
            var model = await Db.Authors.FirstOrDefaultAsync();

            return View(model);
        }

        [Route("/Author/{id}/{slug}")]
        public async Task<IActionResult> Author(int id)
        {
            var model = await Db.Authors.Where(a=> a.Id == id).FirstOrDefaultAsync();

            return View("About", model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact Us";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
