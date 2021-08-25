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
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repository;

        public LivrosController(IRepository<Livro> repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(LivroApi))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ErrorsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ErrorsResponse))]
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
        
        public IActionResult ListarLivros()
        {
            var lista = _repository.All.Select(s=> s.ToApi());
            return Ok(lista);
        }

        [HttpPost]
        public IActionResult Incluir([FromForm] LivroUpload livroUpload)
        {
            if (ModelState.IsValid)
            {
                var livro = livroUpload.ToLivro();
                _repository.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);
            }
            return BadRequest();

        }

        [HttpPut]
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
