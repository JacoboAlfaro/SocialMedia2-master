using SocialMedia.Core.CustomEntities;

namespace SocialMedia.Api.Responses
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data)
        {
            respuestaExitosa = true;
            Data = data;
        }
        public ApiResponse()
        {
            respuestaExitosa = true;
        }
        public string Response { get; set; }
        public bool respuestaExitosa { get; set; }
        public T Data { get; set; }
        public MetaData Meta { get; set; }


    }
}
