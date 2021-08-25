using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelos
{
    public class ErrorsResponse
    {
        public int Codigo { get; set; }

        public string Mensagem { get; set; }

        public string[] Detalhes { get; set; }

        public ErrorsResponse InnerError { get; set; }

        public static ErrorsResponse From(Exception e)
        {

            if (e == null)
            {
                return null;
            }
            return new ErrorsResponse
            {
                Codigo = e.HResult,
                Mensagem = e.Message,
                InnerError = From(e.InnerException)
            };
        }

        public static ErrorsResponse FromModelState(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(s => s.Errors);

            return new ErrorsResponse
            {
                Codigo = 100,
                Mensagem = "Houve erro(s) no envio da requisição.",
                Detalhes = errors.Select(s=> s.ErrorMessage).ToArray()
            };
        }
    }
}
