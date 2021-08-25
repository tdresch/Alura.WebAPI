using Alura.ListaLeitura.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Alura.WebAPI.Api.Modelos
{

    public static class LivroOrdemExtensions
    {

        public static IQueryable<Livro> AplicarOrdem(this IQueryable<Livro> query, LivroOrdem ordem)
        {

            if (!string.IsNullOrEmpty(ordem?.OrdenarPor))
            {

                query = query.OrderBy(ordem.OrdenarPor) ;
            }
            return query;
        }
    }

    public class LivroOrdem
    {
        public string OrdenarPor { get; set; }
    }
}
