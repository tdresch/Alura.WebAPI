using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Formatters
{
    public class LivroCsvFormatter : TextOutputFormatter
    {

        public LivroCsvFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var livroCsv = "";

            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;

                livroCsv = $"{livro.Titulo};{livro.Subtitulo};{livro.Resumo};{livro.Autor};{livro.Lista};";}

            using (var writter = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return writter.WriteAsync(livroCsv);
            };

        }

        protected override bool CanWriteType(Type type)
        {
            return type == typeof(LivroApi);
        }
    }
}
