using ASPNetBlog.App_Common.Mapper;
using ASPNetBlog.ViewComponents;
using Microsoft.AspNet.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/16/15*/

namespace ASPNetBlog.Models
{
    public class CategoryViewModel : Category
    {
        // Important to create empty constructor so that model can be created when its being passed to action 
        public CategoryViewModel() { }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

    }

    public class TagViewModel : Tag
    {
        // Important to create empty constructor so that model can be created when its being passed to action 
        public TagViewModel() { }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

    }

    public class AuthorViewModel : Author
    {
        // Important to create empty constructor so that model can be created when its being passed to action 
        public AuthorViewModel() { }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
    }

    public class PostViewModel : Post
    {
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }

        // created to use bool rather bool?
        [Display(Name ="Comments Allowed")]
        public bool NewCommentAllowed
        {
            get { return CommentAllowed ?? false; }
            set { CommentAllowed = value; }
        }

        [Required, Display(Name ="Post Categories")]
        public IEnumerable<int> PostCategories { get; set; }
        [Display(Name ="Post Tags")]
        public IList<string> PostTags { get; set; }

        public PostViewModel() : base()
        {
            PostCategories = new List<int>();
            PostTags = new List<string>();
        }
    }

    public class PostCommentViewModel : PostComment
    {
        public PostCommentViewModel() { }
    }

    public class PageViewModel : Page
    {
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
    }

    public class DeleteViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public virtual int Id { get; set; }

        [Timestamp]
        public byte[] RowStamp { get; set; }

        public string ItemInfo { get; set; }
    }

    public class SidePanelViewModel
    {
        public SidePanel SidePanel { get; set; }
        public string Heading { get; set; }
        public List<KeyValuePair<string,string>> Data { get; set; }
    }
}
