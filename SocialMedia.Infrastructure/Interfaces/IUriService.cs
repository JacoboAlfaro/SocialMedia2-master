using SocialMedia.Core.QueryFilters;
using System;

namespace SocialMedia.Infrastructure.Interfaces
{
    public interface IUriService
    {
        Uri GetPostPaginationUri(PostQueryFilter filter, string actionUrl, int currentPage);
        Uri GetUserPaginationUri(UserQueryFilter filter, string actionUrl, int currentPage);
        Uri GetCommentPaginationUri(CommentQueryFilter filter, string actionUrl, int currentPage);
        Uri GetLoginsPaginationUri(SecurityQueryFilter filter, string actionUrl, int currentPage);
        Uri GetCategoriesPaginationUri(CategoryQueryFilter filter, string actionUrl, int currentPage);
    }
}