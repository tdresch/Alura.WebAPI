using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Alura.WebAPI.WebApp.HttpClients;
using System.Threading.Tasks;
using System.Linq;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LivroApiClient _livroApiClient;

        public HomeController(LivroApiClient livroApiClient)
        {
            _livroApiClient = livroApiClient;
        }

        private async Task<IEnumerable<LivroApi>> ListaDoTipo(TipoListaLeitura tipo)
        {
            var lista = await _livroApiClient.GetListaleituraAsync(tipo);

            return lista.Livros;
        }

        public async Task<IActionResult> Index()
        {
            

            var model = new HomeViewModel
            {
                ParaLer = await ListaDoTipo(TipoListaLeitura.ParaLer),
                Lendo = await ListaDoTipo(TipoListaLeitura.Lendo),
                Lidos = await ListaDoTipo(TipoListaLeitura.Lidos)
            };
            return View(model);
        }
    }
}