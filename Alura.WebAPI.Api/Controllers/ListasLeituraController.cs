using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Leitura = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ListasLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repository;
        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repository = repository;
        }

        private Leitura CriaLista(TipoListaLeitura tipoListaLeitura)
        {

            return new Leitura
            {
                Tipo = tipoListaLeitura.ParaString(),
                Livros = _repository.All.Where(w => w.Lista == tipoListaLeitura).Select(s => s.ToApi()).ToList()
            };

        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todas as listas de leitura .", Tags = new[] { "Listas" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult TodasListas()
        {

            Leitura paraLer = CriaLista(TipoListaLeitura.ParaLer);
            Leitura lendo = CriaLista(TipoListaLeitura.Lendo);
            Leitura lidos = CriaLista(TipoListaLeitura.Lidos);

            var collection = new List<Leitura> { paraLer, lendo, lidos };

            return Ok(collection);
        }


        [HttpGet("{tipoListaLeitura}")]
        [SwaggerOperation(Summary = "Recupera a lista de leitura identificada por seu {tipo}.", Tags = new[] { "Listas" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult Recuperar([FromRoute][SwaggerParameter("Tipo de lista a ser obtida.")] TipoListaLeitura tipoListaLeitura)
        {
            Leitura lista = CriaLista(tipoListaLeitura);

            return Ok(lista);
        }
    }
}
