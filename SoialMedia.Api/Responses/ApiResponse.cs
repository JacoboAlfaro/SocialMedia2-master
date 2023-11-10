using SocialMedia.Core.CustomEntities;

namespace SocialMedia.Api.Responses
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data)
        {
            Data = data;
        }
        public string Response { get; set; }
        public T Data { get; set; }
        public MetaData Meta { get; set; }


    }
}
