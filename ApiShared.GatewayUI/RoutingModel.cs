using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace ApiShared.Gateway
{
    public static class Datasource
    {
        public static SelectListItem[] DownstreamScheme = { new SelectListItem() { Text = "http", Value = "http" }, new SelectListItem() { Text = "ws", Value = "ws" } };
        public static string[] UpstreamHttpMethod = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        public static List<RoutingViewModel> ConfigData { get; set; } = new List<RoutingViewModel>();
        public static TokenProvider.Token? TokenApi { get; set; }
        public static string EncodeBase64(string str)
        {
            var strBytes = Encoding.UTF8.GetBytes(str);
            var base64 = Convert.ToBase64String(strBytes);
            return base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
        public static string DecodeBase64(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var strBytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(strBytes);
        }
    }

    public class RoutingViewModel
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
    }
}

namespace TokenProvider
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
