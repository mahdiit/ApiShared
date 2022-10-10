using ApiShared.Gateway;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using System.Net.Http.Headers;

namespace ApiShared.GatewayUI.Pages
{
    public class IndexModel : PageModel
    {
        IConfiguration _configuration;
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            if (Datasource.TokenApi == null)
            {
                var sendParam = new Dictionary<string, string>();
                var tokenUrl = _configuration["Gateway"] + "cfgedit/connect/token";
                sendParam.Add("client_id", "admin");
                sendParam.Add("client_secret", "6y4ALTNTey8xueQJPTMfkYNx");
                sendParam.Add("grant_type", "client_credentials");
                sendParam.Add("scope", "admin");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(sendParam));
                    var token = await response.Content.ReadAsStringAsync();

                    Datasource.TokenApi = JsonConvert.DeserializeObject<TokenProvider.Token>(token);
                }
            }

            Datasource.ConfigData = new List<RoutingViewModel>();
            var configUrl = _configuration["Gateway"] + "cfgedit/configuration";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Datasource.TokenApi.AccessToken);

                HttpResponseMessage response = await client.GetAsync(configUrl);
                var result = await response.Content.ReadAsStringAsync();

                FileConfiguration configData  = JsonConvert.DeserializeObject<FileConfiguration>(result);
                var routes = configData.Routes;
                foreach (FileRoute item in routes)
                {
                    var methods = string.Join(',', item.UpstreamHttpMethod);

                    Datasource.ConfigData.Add(new RoutingViewModel()
                    {
                        DownstreamHost = item.DownstreamHostAndPorts.First().Host,
                        DownstreamPort = item.DownstreamHostAndPorts.First().Port.ToString(),
                        DownstreamPathTemplate = item.DownstreamPathTemplate,
                        DownstreamScheme = item.DownstreamScheme,  
                        UpstreamPathTemplate = item.UpstreamPathTemplate,
                        UpstreamHttpMethod = methods
                    });
                }
            }
        }
    }
}