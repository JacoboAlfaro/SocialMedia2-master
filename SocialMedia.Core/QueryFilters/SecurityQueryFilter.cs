using SocialMedia.Core.Enumerations;


namespace SocialMedia.Core.QueryFilters
{
    public class SecurityQueryFilter
    {
        public string User { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoleType? Role { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
