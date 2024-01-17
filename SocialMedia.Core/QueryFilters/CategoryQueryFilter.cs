using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Core.QueryFilters
{
    public class CategoryQueryFilter
    {
        public string Name { get; set; }


        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
