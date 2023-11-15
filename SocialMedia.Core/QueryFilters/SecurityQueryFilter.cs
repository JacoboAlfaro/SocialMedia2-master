using SocialMedia.Core.Enumerations;
using System;


namespace SocialMedia.Core.QueryFilters
{
    public class SecurityQueryFilter
    {
        public string UserLogin { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoleType? Role { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
