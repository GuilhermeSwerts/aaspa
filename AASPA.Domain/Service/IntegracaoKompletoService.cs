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
using System.Threading.Tasks;

namespace AASPA.Domain.Service
{
    public class IntegracaoKompletoService : IIntegracaoKompleto
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _config;
        public IntegracaoKompletoService(IConfiguration config)
        {
            _config = config;
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
                return JsonConvert.DeserializeObject<CancelarPropostaKompletoResponse>(result);
            }
            catch (Exception ex)
            {
                //coreLogger.Register(new { mensagem = ex.Message, request }, LogType.Error, "ApiKompleto/CancelarProposta");
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
