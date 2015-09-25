using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/23/15*/

namespace ASPNetBlog.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        //public DbSet<PublishStatus> PublishStatuses { get; set; }
        //public DbSet<Visibility> Visibility { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            
            foreach (var et in builder.Model.EntityTypes)
            {
                var table = et.Relational().Table;
                if (table.StartsWith("AspNet"))
                    et.Relational().Table = table.Substring(6).TrimEnd('s');
            }

            builder.Entity<ApplicationUser>().Index(x => x.AppUserId).IndexName("IDX_APPUSER_AppUserId");
            builder.Entity<ApplicationUser>().Index(x => x.Email).IndexName("IDX_APPUSER_Email").Unique(true);

            builder.Entity<PostCategory>().Key(k => new { k.PostId, k.CategoryId });
            builder.Entity<PostTag>().Key(k => new { k.PostId, k.TagId });
        }
    }
}
