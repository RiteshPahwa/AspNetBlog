using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

/* MVC 6 Coding Example -- Ritesh Pahwa 9/23/15*/
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles="Admin")] //Should Implement Claims here
    public class HomeController : Controller
    {
        // GET: /<controller>/
        [Route("/Admin")]
        [Route("/Admin/Home")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Coming()
        {
            return View("_ComingSoon");
        }
    }
}
