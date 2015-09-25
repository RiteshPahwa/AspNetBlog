using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Mvc;
using System.IO;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/23/15*/

namespace ASPNetBlog.Models
{

    #region Abstract Model Classes
    public abstract class BlogModel
    {
        public BlogModel() { CreatedOn = DateTime.Now; }

        [HiddenInput(DisplayValue = false)]
        public virtual int Id { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime? CreatedOn { get; set; }
        [ScaffoldColumn(false)]
        public virtual int? CreatedBy { get; set; }
        [ScaffoldColumn(false)]
        public virtual DateTime? ModifiedOn { get; set; }
        [ScaffoldColumn(false)]
        public virtual int? ModifiedBy { get; set; }

        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowStamp { get; set; }
    }

    public abstract class BlogNameSlugModel : BlogModel
    {
        [Required, StringLength(30)]
        public virtual string Name { get; set; }

        [Required, StringLength(30)]
        public virtual string Slug { get; set; }
    }

    public abstract class BlogContentModel : BlogModel
    {
        [Required, StringLength(50)]
        public virtual string Slug { get; set; }

        [Required, Display(Name= "Short Description")]
        [StringLength(500)]
        public virtual string ShortDescription { get; set; }

        [Required(ErrorMessage = "Content is required")]
        //[AllowHtml]
        [DataType(DataType.Html)]
        public virtual string Content { get; set; }
    }
    #endregion

    public class Category : BlogNameSlugModel
    {
        [Display(Name="Parent Category")]
        public virtual int? ParentCategoryId { get; set; }

        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Tag : BlogModel
    {
        [Required, StringLength(30)]
        public virtual string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public class Image : BlogModel
    {
        [Required, StringLength(50)]
        public virtual string Name { get; set; }

        [Required, StringLength(4)]
        public string FileExt { get; set; }

        [DefaultValue(true)]
        public bool OnFileSystem { get; set; }

        [NotMapped]
        public byte[] FileData
        {
            get; set;
            //get { return File.ReadAllBytes(FilePath); }
            //set { File.WriteAllBytes(FilePath, value);}
        }

        [NotMapped]
        public string RelativePath
        {
            get {
                // Get file url path from configuration 
                return $"/BlogImages/{Id.ToString()}.{FileExt}"; 
            }
        }

        protected string FilePath(string fileSystemPath) //IApplicationEnvironment.ApplicationBasePath
        {
            return Path.Combine(fileSystemPath, this.RelativePath);
        }

        protected void DeleteData(string fileSystemPath)
        {
            File.Delete(FilePath(fileSystemPath));
        }

        public byte[] ImageData;
    }

    public class Author : BlogContentModel
    {
        [Required, StringLength(30, ErrorMessage = "Name length cannot exceed 30 chars")]
        public virtual string Name { get; set; }

        [Required, Display(Name="User")]
        public virtual int AppUserId { get; set; }
    }

    public class Page : BlogContentModel
    {
        [Required, StringLength(125, ErrorMessage = "Title length cannot exceed 125 chars")]
        public virtual string Title { get; set; }

        [Required, Display(Name ="Publishing Status")]
        public virtual PublishStatus PublishStatus { get; set; }

        [Required]
        public virtual Visibility Visibility { get; set; }

        public virtual bool? CommentAllowed { get; set; }

        [Required, Display(Name="Author")]
        public virtual int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
    }

    public class Post : BlogContentModel
    {
        [Required, StringLength(125, ErrorMessage = "Title length cannot exceed 125 chars")]
        public virtual string Title { get; set; }

        [Required, Display(Name = "Publish Status")]
        public virtual PublishStatus PublishStatus { get; set; }

        [Required]
        public virtual Visibility Visibility { get; set; }

        [Display(Name = "Comments Allowed?")]
        public virtual bool? CommentAllowed { get; set; }

        [Required, Display(Name = "Author")]
        public virtual int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
        
        [Required, Display(Name = "Posted Date Time")]
        public virtual DateTime PostedOn { get; set; }

        public virtual ICollection<PostCategory> Categories { get; set; }
        public virtual ICollection<PostTag> Tags { get; set; }

        public Post()
        {
            Categories = new List<PostCategory>();
            Tags = new List<PostTag>();
        }
    }

    public class PostCategory
    {
        [Required(ErrorMessage = "PostId is required")]
        public virtual int PostId { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public virtual int CategoryId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Category Category { get; set; }

    }

    public class PostTag
    {
        [Required(ErrorMessage = "PostId is required")]
        public virtual int PostId { get; set; }

        [Required(ErrorMessage = "TagId is required")]
        public virtual int TagId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }

    public class PostComment : BlogModel
    {
        [Required, StringLength(50), Display(Name = "Name")]
        public string Name { get; set; }

        [Required, StringLength(2500), Display(Name = "Comment"), DataType(DataType.Html)]
        public string Comment { get; set; }

        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        [Required, StringLength(50), Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Comment Status")]
        public virtual CommentStatus CommentStatus { get; set; }

        [Display(Name = "Parent Comment")]
        public virtual int? ParentCommentId { get; set; }

        [HiddenInput]
        public virtual int PostId { get; set; }
        public virtual Post Post { get; set; }
    }

    public class Widget : BlogModel
    {
        [Required, StringLength(30, ErrorMessage = "Name length cannot exceed 30 chars")]
        public virtual string Name
        { get; set; }

        [Required(ErrorMessage = "Content Type is required")]
        public virtual ContentType ContentType
        { get; set; }

        public virtual int? ContentTypeId
        { get; set; }

        public virtual int? MaxDisplayCount
        { get; set; }

        public virtual string Content
        { get; set; }

        public virtual int? ParentWidgetId
        { get; set; }
    }

    public class PageWidget
    {
        [Required(ErrorMessage = "PageId is required")]
        public virtual int PageId { get; set; }

        [Required(ErrorMessage = "WidgetId is required")]
        public virtual int WidgetId { get; set; }

        [Required(ErrorMessage = "Widget Location is required")]
        public virtual WidgetLocation WidgetLocation { get; set; }

        public virtual int AttachToWidgetId { get; set; }
        public virtual WidgetPosition AttachToWidgetPosition { get; set; }

        [Required, Display(Name = "Widget Order"), DefaultValue(0)]
        public virtual int WidgetOrder { get; set; }
    }

    public enum Visibility { Public = 1, Private }

    public enum PublishStatus { Draft = 1,  WaitingForReview, Published, Deleted }

    public enum CommentStatus { WaitingForReview = 1, Published, Spam, Deleted }

    public enum ContentType
    {
        Post = 1, Page, Link, Navigation, Html,
        RecentPosts, RelatedPosts, Pages, Categories, TagCloud,
        ContactUs
    }

    public enum WidgetLocation
    { Top = 1, Middle, Bottom, Navigation, Navigation2, Header, Footer, AfterFooter}

    public enum WidgetPosition {Left = 1, Middle, Right, Above, Below }
    

}
