using ASPNetBlog.App_Common.Mapper;
using ASPNetBlog.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/22/15*/

namespace ASPNetBlog.Business
{
    //ToDo This code needs improvement on including entities, 
    //right now Categories and Tags are not being fetched in single query
    //Also projecting into ViewModel drops the includes as well
    public class PostData : IData<Post, int>
    {
        //[FromServices]
        public ApplicationDbContext Db { get; set; }
        public int PageCount { get; private set; }

        protected const int PostPagingSize = 6;

        public PostData(){}
        public PostData(ApplicationDbContext db){ Db = db; }

        public async Task<Post> GetByIdAsync(int id)
        {
            var result = await Db.Posts.Include(p => p.Author).Where(p => p.Id == id)
                .ToListAsync();

            result = await LoadCategoriesAndTagsAsync(result);

            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<Post>> LatestAsync(int pageNo)
        {
            PageCount = await Db.Posts.CountAsync();
            if(PageCount == 0 ) return null;
            PageCount = (PageCount-1) / PostPagingSize + 1;

            var result = await Db.Posts.Include(p=> p.Author)
                .OrderByDescending(p => p.PostedOn).Skip((pageNo - 1) * PostPagingSize).Take(PostPagingSize)
                .DefaultIfEmpty().ToListAsync();

            result.All(p => { p.Content = null; return true; }); // This is best for Deselecting so far :(
            result = await LoadCategoriesAndTagsAsync(result);

            return result;
        }

        public async Task<IEnumerable<Post>> GetByCategoryAsync(int id, int pageNo)
        {
            PageCount = await Db.PostCategories.Where(c => c.CategoryId == id).CountAsync();
            if (PageCount == 0) return null;
            PageCount = (PageCount - 1) / PostPagingSize + 1;

            var result = await Db.Posts.Include(p => p.Author).Include(p => p.Categories)
                .Where(p => p.Categories.Any(c => c.CategoryId == id))
                .OrderByDescending(p => p.PostedOn).Skip((pageNo - 1) * PostPagingSize).Take(PostPagingSize)
                .DefaultIfEmpty().ToListAsync();

            result.All(p => { p.Content = null; return true; }); // This is best for Deselecting so far :(
            result = await LoadCategoriesAndTagsAsync(result);

            return result;
        }


        public async Task<IEnumerable<Post>> GetByTagAsync(int id, int pageNo)
        {
            PageCount = await Db.PostTags.Where(t => t.TagId == id).CountAsync();
            if (PageCount == 0) return null;
            PageCount = (PageCount - 1) / PostPagingSize + 1;

            var result = await Db.Posts.Include(p => p.Author).Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.TagId == id))
                .OrderByDescending(p => p.PostedOn).Skip((pageNo - 1) * PostPagingSize).Take(PostPagingSize)
                .DefaultIfEmpty().ToListAsync();

            result.All(p => { p.Content = null; return true; }); // This is best for Deselecting so far :(
            result = await LoadCategoriesAndTagsAsync(result);

            return result;
        }

        public async Task<IEnumerable<Post>> SearchAsync(string searchText, int pageNo)
        {
            PageCount = await Db.Posts
                .Where(p => p.Title.ToUpper().Contains(searchText.ToUpper()) || p.ShortDescription.ToUpper().Contains(searchText.ToUpper()))
                .CountAsync();
            if (PageCount == 0) return null;
            PageCount = (PageCount - 1) / PostPagingSize + 1;

            var result = await Db.Posts.Include(p => p.Author)
                .Where(p => p.Title.ToUpper().Contains(searchText.ToUpper()) || p.ShortDescription.ToUpper().Contains(searchText.ToUpper()))
                .OrderByDescending(p => p.PostedOn).Skip((pageNo - 1) * PostPagingSize).Take(PostPagingSize)
                .DefaultIfEmpty().ToListAsync();

            result.All(p => { p.Content = null; return true; }); // This is best for Deselecting so far :(
            result = await LoadCategoriesAndTagsAsync(result);

            return result;
        }

        public async Task<List<Post>> LoadCategoriesAndTagsAsync(List<Post> posts)
        {
            foreach (var post in posts)
            {
                if (post == null) break;
                post.Categories = await Db.PostCategories.Include(c => c.Category).Where(c => c.PostId == post.Id).ToListAsync();
                post.Tags = await Db.PostTags.Include(c => c.Tag).Where(c => c.PostId == post.Id).ToListAsync();
            }
            return posts;
        }

    }
}
