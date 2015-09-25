using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Framework.OptionsModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/16/15*/

namespace ASPNetBlog.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public virtual string SecondEmail { get; set; }

        [Required, MaxLength(50), DisplayName("Full Name")]
        public virtual string FullName { get; set; }

        // Used it for storing on CreadedBy or ModifiedBy or anything similar for DB Joins, etc.
        [ScaffoldColumn(false), DefaultValue(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int? AppUserId { get; set; }
        public virtual bool? Activated { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }

        [MaxLength(125)]
        public virtual string Description { get; set; }
    }

}
