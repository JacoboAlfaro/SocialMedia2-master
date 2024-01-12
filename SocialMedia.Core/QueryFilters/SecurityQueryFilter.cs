using SocialMedia.Core.Enumerations;
using System;


namespace SocialMedia.Core.QueryFilters
{
    public class SecurityQueryFilter
    {
        public string UserLogin { get; set; }
        public string UserName { get; set; }
        public RoleType? Role { get; set; }

        //QUERYS USER INFO
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public bool? IsActive { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
