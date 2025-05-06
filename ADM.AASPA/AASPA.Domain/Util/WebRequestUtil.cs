using AASPA.Models.Enums;
using AASPA.Models.helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AASPA.Domain.Util
{
    public class WebRequestUtil
    {
        public static class Integrral
        {
            private static readonly IConfiguration _configuration;

            static Integrral()
            {
                _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            }

            public static async Task<string> GerarToken()
            {
                try
                {
                    HttpClient _httpClient = new();

                    var requestUriLogin = _configuration["IntegraallApi:BaseUrl"] + "api/Login/validar";
                    var loginRequest = new
                    {
                        login = _configuration["IntegraallApi:login"].ToString(),
                        senha = _configuration["IntegraallApi:senha"].ToString(),
                        captcha = _configuration["IntegraallApi:captcha"].ToString(),
                        token = _configuration["IntegraallApi:token"].ToString()
                    };

                    var json = JsonConvert.SerializeObject(loginRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(requestUriLogin, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                        return responseObject.token;
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao tentar gerar token.", ex);
                }
            }

            public async static Task<bool> Post(string body, EEndpointsIntegraall    endpoint)
            {
                try
                {
                    var url = _configuration["IntegraallApi:BaseUrl"] + endpoint.GetDescription();
                    using var client = new HttpClient();

                    var token = await GerarToken();

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var jsonBody = (body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);        

                    if (!response.IsSuccessStatusCode)
                    {
                        string erroDetalhado = await response.Content.ReadAsStringAsync();
                    }

                    return response.IsSuccessStatusCode;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async static Task<Return> Post<Return>(string body, EEndpointsIntegraall endpoint)
            {
                try
                {
                    var client = new RestClient(_configuration["IntegraallApi:BaseUrl"] + endpoint.GetDescription());

                    var request = new RestRequest("", Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", $"Bearer {GerarToken()}");

                    request.AddJsonBody(body);

                    var response = await client.ExecuteAsync<Return>(request);

                    if (!response.IsSuccessful)
                    {
                        return default;
                    }

                    return response.Data;
                }
                catch (Exception)
                {

                    throw;
                }

            }

        }
    }
}
