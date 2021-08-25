using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;

namespace Alura.WebAPI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class Livros2Controller : ControllerBase
    {
        private readonly IRepository<Livro> _repository;

        public Livros2Controller(IRepository<Livro> repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera o livro identificado por seu {id}.", Tags = new[] { "Livros" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult Recuperar(int id)
        {
            var model = _repository.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToApi());
        }

        [HttpGet("{id}/capa")]
        [SwaggerOperation(
            Summary = "Recupera a capa do livro identificado por seu {id}.",
            Tags = new[] { "Livros" },
            Produces = new[] { "image/png" }
        )]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repository.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera uma coleção paginada de livros.", Tags = new[] { "Livros" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroPaginado))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ErrorsResponse))]
        public IActionResult ListarLivros([FromQuery] LivroFiltro filtro, [FromQuery] LivroOrdem ordem, [FromQuery] LivroPaginacao paginacao)
        {
            var lista = _repository.All
                .AplicarFiltro(filtro)
                .AplicarOrdem(ordem)
                .Select(s => s.ToApi())
                .ToLivroPaginado(paginacao);
            return Ok(lista);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Registra novo livro na base.", Tags = new[] { "Livros" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ErrorsResponse))]
        public IActionResult Incluir([FromForm, SwaggerParameter("Titulo", Required = true)] LivroUpload livroUpload)
        {
            if (ModelState.IsValid)
            {
                var livro = livroUpload.ToLivro();
                _repository.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);
            }
            return BadRequest(ErrorsResponse.FromModelState(ModelState));

        }

        [HttpPut]
        [SwaggerOperation(Summary = "Altera um livro na base.", Tags = new[] { "Livros" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ErrorsResponse))]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repository.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repository.Alterar(livro);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui um livro na base de dados a partir do seu {id}.", Tags = new[] { "Livros" }, Produces = new[] { "application/json", "application/xml" })]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ErrorsResponse))]
        public IActionResult Excluir(int id)
        {
            var model = _repository.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repository.Excluir(model);
            return NoContent();
        }
    }
}
