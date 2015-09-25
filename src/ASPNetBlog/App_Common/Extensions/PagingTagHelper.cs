using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/21/15*/

namespace ASPNetBlog.App_Common.Extensions
{
    // In pageUrl pageNo placeholder is {p}, which will be replaced with number
    [TargetElement(Attributes = "PageCount,PageUrl")]
    public class PagingTagHelper : TagHelper
    {
        [HtmlAttributeName(nameof(PageNo))]
        public int PageNo { get; set; } = 1;
        [HtmlAttributeName(nameof(PageCount))]
        public int PageCount { get; set; }
        [HtmlAttributeName(nameof(PagingWindow))]
        public int PagingWindow { get; set; } = 10;
        [HtmlAttributeName(nameof(PageUrl))]
        public string PageUrl { get; set; }
        [HtmlAttributeName(nameof(PageUrlTitle))]
        public string PageUrlTitle { get; set; }
        [HtmlAttributeName(nameof(PageHtml))]
        public string PageHtml { get; set; }
        [HtmlAttributeName(nameof(PagePrevHtml))]
        public string PagePrevHtml { get; set; } = "&laquo;";
        [HtmlAttributeName(nameof(PageNextHtml))]
        public string PageNextHtml { get; set; } = "&raquo;";

        private int pagingWindowHalf { get; set; } = 5; 
        private int startPage { get; set; } = 1;
        private int endPage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {            
            if(string.IsNullOrWhiteSpace(PageUrlTitle)) PageUrlTitle = "View page {p}";

            pagingWindowHalf = PagingWindow / 2;
            if (PageNo < 2) PageNo = 1;
            if (PageNo > PageCount) PageNo = PageCount;

            // Adjust the paging window, on where to start and end page 
            if (PageNo > pagingWindowHalf && PageCount > PagingWindow)
            {
                startPage = PageNo - pagingWindowHalf + 1;
                if ((PageCount - PageNo) < pagingWindowHalf) startPage = startPage - (pagingWindowHalf - (PageCount - PageNo));
                endPage = startPage + PagingWindow -1;
            } else
            {
                endPage = PageCount;
            }

            var pageElements = new StringBuilder();

            output.TagName = "ul";
            for (var i = startPage; i <= endPage; i++)
            {
                if (i == startPage && !string.IsNullOrWhiteSpace(PagePrevHtml))
                    pageElements.AppendLine(buildPageElement(i, PagePrevHtml));

                pageElements.AppendLine(buildPageElement(i, PageHtml, i == PageNo ? "active" : ""));

                if (i == endPage && !string.IsNullOrWhiteSpace(PageNextHtml))
                    pageElements.AppendLine(buildPageElement(i, PageNextHtml));
            }

            output.Content.SetContent(pageElements.ToString());
            //Beautiful C# 6.0 says it in one line below
            output.Attributes["class"] = $"pagination {output.Attributes["class"]?.Value}";
        }

        private string buildPageElement(int currentPageNo, string aHtml, string cssClass = null)
        {
            var li = new TagBuilder("li");

            var anchor = new TagBuilder("a");
            anchor.MergeAttribute("href", PageUrl.Replace("{p}", $"{currentPageNo}"));
            anchor.MergeAttribute("title", PageUrlTitle.Replace("{p}", $"{currentPageNo}"));

            var anchorHtml = aHtml?.Replace("{p}", $"{currentPageNo}");
            if (string.IsNullOrWhiteSpace(anchorHtml)) anchorHtml = $"{currentPageNo}";
            anchor.InnerHtml = anchorHtml;

            li.InnerHtml = anchor.ToString();
            if (!string.IsNullOrWhiteSpace(cssClass)) li.AddCssClass(cssClass);
            return li.ToString();

        }
    }
}
