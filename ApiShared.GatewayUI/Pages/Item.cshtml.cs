using ApiShared.Gateway;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiShared.GatewayUI.Pages
{
    public class ItemModel : PageModel
    {
        public string Id
        {
            get
            {
                return Datasource.EncodeBase64($"{DownstreamScheme}://{DownstreamHost}:{DownstreamPort}{DownstreamPathTemplate}");
            }
        }

        public string DownstreamPathTemplate { get; set; }
        public string DownstreamScheme { get; set; }
        public string DownstreamHost { get; set; }
        public string DownstreamPort { get; set; }
        public string UpstreamPathTemplate { get; set; }
        public string UpstreamHttpMethod { get; set; }

        public void OnGet()
        {
            var mode = Request.Query["mode"];
            var id = Request.Query["id"];

            if(mode == "Edit")
            {
                var item = Datasource.ConfigData.First(x => x.Id == id);
                DownstreamHost = item.DownstreamHost;
                DownstreamPathTemplate = item.DownstreamPathTemplate;
                DownstreamPort = item.DownstreamPort;
                DownstreamScheme = item.DownstreamScheme;

                UpstreamPathTemplate = item.UpstreamPathTemplate;
                UpstreamHttpMethod = item.UpstreamHttpMethod;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            return Redirect("Index");
        }
    }
}
