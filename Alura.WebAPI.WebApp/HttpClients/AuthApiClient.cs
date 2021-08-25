using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.WebAPI.WebApp.HttpClients
{

    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }

    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> PostLoginAsync(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("login", loginModel);
            return new LoginResult
            {
                Succeeded = response.IsSuccessStatusCode,
                Token = await response.Content.ReadAsStringAsync()
            };
        }

    }
}
