using ASPNetBlog.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetBlog.ViewComponents
{
    public class CommentViewComponent : ViewComponent
    {
        [FromServices]
        public ApplicationDbContext Db { get; set; }

        public CommentViewComponent(ApplicationDbContext context)
        {
            Db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            if (id == 0) return View(null);

            var cmnts = await Db.PostComments.Where(p => p.PostId == id).ToListAsync();

            return View(cmnts);
        }
    }
}
