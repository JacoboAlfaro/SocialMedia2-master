using SocialMedia.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
        public int Likes { get; set; }

        public virtual PostDto Post { get; set; }
        public virtual UserDto User { get; set; }
    }
}
