using SocialMedia.Core.QueryFilters;
using SocialMedia.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPostPaginationUri(PostQueryFilter filter, string actionUrl, int currentPage)
        {
            string baseUrl = $"{_baseUri}{actionUrl}?pageNumber={currentPage}";
            return new Uri(baseUrl);
        }
        public Uri GetUserPaginationUri(UserQueryFilter filter, string actionUrl, int currentPage)
        {
            string baseUrl = $"{_baseUri}{actionUrl}?pageNumber={currentPage}";
            return new Uri(baseUrl);
        }
        public Uri GetCommentPaginationUri(CommentQueryFilter filter, string actionUrl, int currentPage)
        {
            string baseUrl = $"{_baseUri}{actionUrl}?pageNumber={currentPage}";
            return new Uri(baseUrl);
        }
    }
}
