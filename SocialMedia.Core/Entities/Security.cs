using SocialMedia.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.Entities
{
    public class Security: BaseEntity
    {
        public string UserLogin { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public RoleType Role { get; set; }

        public virtual User User { get; set; }

    }
}
