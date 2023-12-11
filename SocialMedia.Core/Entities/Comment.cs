using System;

namespace SocialMedia.Core.Entities
{
    public partial class Comment : BaseEntity
    {

        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public bool IsEdit { get; set; }
        public int Likes { get; set; }


        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
