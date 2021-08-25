using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Leitura = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.WebApp.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddBearerToken()
        {
            var token = _httpContextAccessor.HttpContext.User.Claims.First(f => f.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<Leitura> GetListaleituraAsync(TipoListaLeitura tipoListaLeitura)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"ListasLeitura/{tipoListaLeitura}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Leitura>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();

        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.DeleteAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultiPartFormdataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Titulo), "\"titulo\"");
            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), "\"subtitulo\"");
            }
            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), "\"resumo\"");
            }
            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), "\"autor\"");
            }
            content.Add(new StringContent(model.Lista.ParaString()), "\"lista\"");
            if (model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(imageContent, "\"capa\"", "\"capa.png\"");
            }
            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), "\"id\"");
            }

            return content;
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultiPartFormdataContent(model);
            HttpResponseMessage response = await _httpClient.PostAsync($"livros/", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            HttpContent content = CreateMultiPartFormdataContent(model);
            HttpResponseMessage response = await _httpClient.PutAsync($"livros/", content);
            response.EnsureSuccessStatusCode();
        }

    }
}
