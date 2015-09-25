using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Framework.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/23/15*/

namespace ASPNetBlog.Models
{
    public static class SampleData
    {
        const string blogAdminUserName = "BlogAdminUserName";
        const string blogAdminPassword = "blogAdminPassword";

        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool createUsers = true)
        {
            using (var db = serviceProvider.GetService<ApplicationDbContext>())
            {
                if (await db.Database.EnsureCreatedAsync())
                {
                    await InsertTestData(serviceProvider);
                    if (createUsers)
                    {
                        const string adminRole = "Admin";
                        var roleExists = await db.Roles.AnyAsync(r => r.Name == adminRole);
                        if (!roleExists)
                        {
                            db.Roles.Add(new IdentityRole { Name = adminRole, NormalizedName = adminRole });
                            await db.SaveChangesAsync();
                        }
                        await CreateAdminUser(serviceProvider);
                    }
                }
            }
        }

        private static async Task InsertTestData(IServiceProvider serviceProvider)
        {

            using (var db = serviceProvider.GetService<ApplicationDbContext>())
            {
                var authors = Authors();
                var cats = Categories();
                var tags = Tags();

                if (!db.Authors.Any())
                    db.Authors.AddRange(authors);
                if (!db.Categories.Any())
                    db.Categories.AddRange(cats);
                if (!db.Tags.Any())
                    db.Tags.AddRange(tags);

                await db.SaveChangesAsync();
                var posts = GetPosts(authors.First().Id);
                if (!db.Posts.Any())
                    db.Posts.AddRange(posts);

                await db.SaveChangesAsync();
                foreach(var post in posts)
                {
                    foreach (var cat in cats)
                        db.PostCategories.Add(new PostCategory { PostId = post.Id, CategoryId = cat.Id });
                    foreach (var tag in tags)
                        db.PostTags.Add(new PostTag { PostId = post.Id, TagId = tag.Id });
                }
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a store manager user who can manage the inventory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            var appEnv = serviceProvider.GetService<IApplicationEnvironment>();

            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables();
            var configuration = builder.Build();

            const string adminRole = "Admin";

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            // TODO: Identity SQL does not support roles yet
            //var roleManager = serviceProvider.GetService<ApplicationRoleManager>();
            //if (!await roleManager.RoleExistsAsync(adminRole))
            //{
            //    await roleManager.CreateAsync(new IdentityRole(adminRole));
            //}

            var user = await userManager.FindByNameAsync(configuration.Get(blogAdminUserName));
            if (user == null)
            {
                user = new ApplicationUser { UserName = configuration.Get(blogAdminUserName) };
                await userManager.CreateAsync(user, configuration.Get(blogAdminPassword));
                await userManager.AddToRoleAsync(user, adminRole);
                await userManager.AddClaimAsync(user, new Claim("ManageBlog", "Allowed"));
            }

#if TESTING
            var envPerfLab = configuration.Get("PERF_LAB");
            if (envPerfLab == "true")
            {
                for (int i = 0; i < 100; ++i)
                {
                    var email = string.Format("User{0:D3}@example.com", i);
                    var normalUser = await userManager.FindByEmailAsync(email);
                    if (normalUser == null)
                    {
                        await userManager.CreateAsync(new ApplicationUser { UserName = email, Email = email }, "Password~!1");
                    }
                }
            }
#endif
        }

        private static Post[] GetPosts(int authorId)
        {
            var posts = new Post[]
            {
                new Post() { Title = "The Best Of The Men At Work 1", AuthorId = authorId, ShortDescription = "Short Description, please modify", Content = "Post content goes gere, please modify" },
                new Post() { Title = "The Best Of The Men At Work 2", AuthorId = authorId, ShortDescription = "Short Description, please modify", Content = "Post content goes gere, please modify" },
                new Post() { Title = "The Best Of The Men At Work 3", AuthorId = authorId, ShortDescription = "Short Description, please modify", Content = "Post content goes gere, please modify" }
            };

            return posts;
        }

        public static List<Category> Categories()
        {
            var list = new List<Category>
            {
                new Category { Name = "Asp.Net", Slug = "asp-net" },
                new Category { Name = "MVC", Slug = "mvc" }
            };

            return list;
        }

        public static List<Tag> Tags()
        {
            var list = new List<Tag>
            {
                new Tag { Name = "Asp.Net 5"},
                new Tag { Name = "MVC 6" },
                new Tag { Name = "jQuery" },
                new Tag { Name = "Bootstrap" },
                new Tag { Name = "Select2" },
                new Tag { Name = "Summernote" },
                new Tag { Name = "SB Admin 2" }
            };

            return list;
        }

        public static List<Author> Authors()
        {
            var list = new List<Author>() {
                new Author { Name = "Ritesh pahwa", Slug = "ritesh-pahwa", AppUserId = 1 }
            };

            return list;
        }
    
    }
 }
