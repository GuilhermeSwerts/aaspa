using AASPA.Domain.Interface;
using AASPA.Models.Requests;
using AASPA.Models.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class IntegracaoKompletoService : IIntegracaoKompleto
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _config;
        private readonly ILogCancelamento _log;
        public IntegracaoKompletoService(IConfiguration config, ILogCancelamento log)
        {
            _config = config;
            _log = log;
        }
        public async Task<CancelarPropostaKompletoResponse> CancelarPropostaAsync(AlterarStatusClientesIntegraallRequest request)
        {
            try
            {
                var apikey = _config["IntegracaoKompleto:ApiKey"];
                var cpfSolicitante = _config["IntegracaoKompleto:CpfSolicitante"];
                var url = _config["IntegracaoKompleto:BaseUrl"];

                var cancKompleto = new CancelarPropostaKompletoRequest
                {
                    cpfSolicitante = cpfSolicitante,
                    token = request.token,
                    motivoCancelamento = request.motivocancelamento
                };

                _httpClient.DefaultRequestHeaders.Add("KKAPIKEY", apikey);
                _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(cancKompleto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _log.Logger(request, "Kompleto", (int)response.StatusCode, JsonDocument.Parse(result).RootElement.GetProperty("message").GetString());
                }
                else
                {
                    _log.Logger(request, "Kompleto", (int)response.StatusCode, "");
                }                
                return JsonConvert.DeserializeObject<CancelarPropostaKompletoResponse>(result);
            }
            catch (Exception ex)
            {
                _log.Logger(request, "Kompleto", 500, ex.Message);
                return new CancelarPropostaKompletoResponse()
                {
                    Ok = false,
                    Message = ex.Message,
                    Title = "",
                    Errors = new List<string>()
                };
            }
        }
    }
}
