using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.DTOs
{
    class PostCommentsCountDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ImageFIle? Image { get; set; }
        public bool IsEdit { get; set; }
        public int Likes { get; set; }

        public virtual UserDto User { get; set; }
        public int c { get; set; }
    }
}
