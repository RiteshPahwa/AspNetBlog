using ASPNetBlog.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetBlog.ViewComponents
{
    public enum ChartType { AREA, BAR, DONUT }

    public class ChartViewComponent : ViewComponent
    {
        //[FromServices]
        public ApplicationDbContext Db { get; set; }

        public ChartViewComponent(ApplicationDbContext context)
        {
            Db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string chartType, int days)
        {
            if (days == 0) return Json("");
            chartType = chartType.ToUpper();
            // ToDo Replace it with some sensible data queries
            if (chartType == nameof(ChartType.AREA)) return AreaChartData(days);
            else if (chartType == nameof(ChartType.BAR)) return BarChartData(days);
            else if (chartType == nameof(ChartType.DONUT)) return DonutChartData(days);

            return Json(""); 
        }

        private JsonViewComponentResult AreaChartData(int days)
        {
            //var cmnts = await Db.PostComments.Where(p => p.PostId == id).ToListAsync();
            var period = Enumerable.Range(1, 10).Select(x => $"{2012 + x / 4} Q{(x % 4) + 1}");
            var iphone = Enumerable.Range(4, 10).Select(x => x * 1000);
            var ipad = Enumerable.Range(4, 10).Select(x => 15000 - x * (x % 4 + 1) * 400);
            var itouch = Enumerable.Range(4, 10).Select(x => 8000 - x * 600);

            var result = period.Zip(iphone, (a, b) => new { a, b })
                .Zip(ipad, (a, c) => new { a.a, a.b, c })
                .Zip(itouch, (a, d) => new { period = a.a, iphone = a.b, ipad = a.c, itouch = d });

            return Json(result);
        }

        private JsonViewComponentResult BarChartData(int days)
        {
            var period = Enumerable.Range(2007, 7).Select(x => $"{x}");
            var iphone = Enumerable.Range(4, 7).Select(x => x + x * 2);
            var ipad = Enumerable.Range(4, 7).Select(x => 50 - x * (x % 2 + 1) * 3);

            var result = period.Zip(iphone, (y, a) => new { y , a }).Zip(ipad, (x, b) => new { x.y, x.a, b });
            return Json(result);
        }

        private JsonViewComponentResult DonutChartData(int days)
        {
            var period = Enumerable.Range(1, 10).Select(x => $"{2012 + x / 4} Q{(x % 4) + 1}");
            var iphone = Enumerable.Range(4, 10).Select(x => x * 2000);

            var result = period.Zip(iphone, (a, b) => new { label = a, value = b });
            return Json(result);
        }
    }
}
