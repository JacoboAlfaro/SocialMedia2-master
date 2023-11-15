using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialMedia.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SocialMedia.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context) 
        {

            //Excepcion BadRequest
            if (context.Exception.GetType() == typeof(BusinessExceptions))
            {
                var exception = (BusinessExceptions)context.Exception;
                var json = new
                {
                    respuestaExitosa = false,
                    mensaje = "Error con el proceso",
                    errors = new
                    {
                        Status = 400,
                        Tittle = "Bad Request",
                        Detail = exception.Message
                    }
            };
                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ExceptionHandled = true;
            }

            //Excepcion NotFound
            if (context.Exception.GetType() == typeof(NotFoundExceptions))
            {
                var exception = (NotFoundExceptions)context.Exception;
                var json = new
                {
                    respuestaExitosa = false,
                    mensaje = "No encontrado",
                    errors = new
                    {
                        Status = 404,
                        Tittle = "Not Found",
                        Detail = exception.Message
                    }
                };
                context.Result = new NotFoundObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.ExceptionHandled = true;
            }
        }
    }
}
