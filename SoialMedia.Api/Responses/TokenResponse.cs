using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Api.Responses
{
    public class TokenResponse {
        public TokenResponse(string token, Security usuarioLogado)
        {
            RespuestaExitosa = true;
            Mensaje = "Operacion exitosa!";
            Token = token;
            UsuarioLogado = usuarioLogado;
            
        }

        public bool RespuestaExitosa { get; set; }
        public string Mensaje { set; get; }
        public string Token { set; get; }
        public Security UsuarioLogado { set; get; }

    }
}
