using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;

namespace SocialMedia.Core.DTOs
{
    public class SecurityDto
    {
        public int Id { get; set; }
        public string UserLogin { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoleType? Role { get; set; }

        public virtual UserDto User { get; set; }

    }
}
