using System;

namespace SocialMedia.Core.QueryFilters
{
    public class UserQueryFilter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsActive { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
