using ASPNetBlog.App_Common.Extensions;
using ASPNetBlog.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetBlog.ViewComponents
{
    public enum SidePanel
    { PostRecent, PostArchive, PostRecentlyViewed, PostMostViewed, PostNextPrev, PostNext, Categories, Tags }

    public class SidePanelViewComponent : ViewComponent
    {
        //[FromServices]
        public ApplicationDbContext Db { get; set; }

        public SidePanelViewComponent(ApplicationDbContext context)
        {
            Db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(SidePanel sidePanel)
        {           
            if (sidePanel == SidePanel.PostRecent) return await PostRecentAsync(7);
            if (sidePanel == SidePanel.Categories) return await CategoriesAsync(10);
            if (sidePanel == SidePanel.Tags) return await TagsAsync(31);

            return Content("");
        }

        private async Task<IViewComponentResult> PostRecentAsync(int count)
        {
            var posts = await Db.Posts.OrderByDescending(p => p.PostedOn)
                .Select(p=> new KeyValuePair<string, string>(p.Title, $"/Post/{p.Id}/{p.Slug}"))
                .Take(count).ToListAsync();
            var sidePanelObj = new SidePanelViewModel{ SidePanel = SidePanel.PostRecent, Heading = "Recent Posts", Data = posts };
            return View(sidePanelObj);
        }
        private async Task<IViewComponentResult> CategoriesAsync(int count)
        {
            var categories = await Db.Categories
                .Select(c => new KeyValuePair<string, string>(c.Name, $"/Category/{c.Id}/{c.Slug}"))
                .Take(count).ToListAsync();
            var sidePanelObj = new SidePanelViewModel { SidePanel = SidePanel.Categories, Heading = "Categories", Data = categories };
            return View(sidePanelObj);
        }
        private async Task<IViewComponentResult> TagsAsync(int count)
        {
            var tags = await Db.Tags
                .Select(t => new KeyValuePair<string,string>(t.Name, $"/Post/{t.Id}/{t.Name.ToSlug()}"))
                .Take(count).ToListAsync();
            var sidePanelObj = new SidePanelViewModel { SidePanel = SidePanel.Tags, Heading = "Tags", Data = tags };
            return View(sidePanelObj);
        }
    }
}
