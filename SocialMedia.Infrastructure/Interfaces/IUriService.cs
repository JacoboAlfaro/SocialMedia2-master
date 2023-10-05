using SocialMedia.Core.QueryFilters;
using System;

namespace SocialMedia.Infrastructure.Interfaces
{
    public interface IUriService
    {
        Uri GetPostPaginationUri(PostQueryFilter filter, string actionUrl);
        Uri GetUserPaginationUri(UserQueryFilter filter, string actionUrl);

    }
}