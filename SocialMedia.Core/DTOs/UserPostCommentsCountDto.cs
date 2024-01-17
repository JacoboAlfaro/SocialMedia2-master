using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.DTOs
{
    public class UserPostCommentsCountDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public bool? IsActive { get; set; }
        public int Followers { get; set; }

        public virtual ICollection<CommentDto> Comments { get; set; }
        public virtual ICollection<PostDto> Posts { get; set; }
    }
}
