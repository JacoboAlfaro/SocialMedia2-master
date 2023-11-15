using SocialMedia.Core.CustomEntities;

namespace SocialMedia.Api.Responses
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data)
        {
            RespuestaExitosa = true;
            Mensaje = "Operación exitosa";
            Data = data;
        }
        public ApiResponse()
        {
            RespuestaExitosa = true;
        }
        public bool RespuestaExitosa { get; set; }
        public string Mensaje { set; get; }
        public T Data { get; set; }
        public MetaData Meta { get; set; }


    }
}
