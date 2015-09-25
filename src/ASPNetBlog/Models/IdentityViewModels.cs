using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.Models
{
    public class RoleViewModel
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        [MaxLength(125)]
        public string Description { get; set; }

        public RoleViewModel() { }
        public RoleViewModel(IdentityRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
            this.ConcurrencyStamp = role.ConcurrencyStamp;
            //this.Description = role.Description;
        }

    }

    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }

        public SelectRoleEditorViewModel(ApplicationRole role)
        {
            this.RoleName = role.Name;

            // Assign the new Descrption property:
            this.Description = role.Description;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Add the new Description property:
        public string Description { get; set; }
    }

    public class EditRoleViewModel
    {
        public string OriginalRoleName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public EditRoleViewModel() { }
        public EditRoleViewModel(ApplicationRole role)
        {
            this.OriginalRoleName = role.Name;
            this.RoleName = role.Name;
            this.Description = role.Description;
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public int AccessFailedCount { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string NormalizedEmail { get; set; }
        public string NormalizedUserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public int? AppUserId { get; set; }
        public bool? Activated { get; set; }

        [Display(Name="User Roles")]
        public IEnumerable<string> UserRoles { get; set; }

        public UserViewModel() { }

    }

}
