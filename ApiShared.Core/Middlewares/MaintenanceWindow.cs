using System.Text;

namespace ApiShared.Core.Middlewares
{
    public class MaintenanceWindow
    {

        private Func<bool> enabledFunc;
        private byte[] response;

        public MaintenanceWindow(Func<bool> enabledFunc, byte[]? response = null)
        {
            this.enabledFunc = enabledFunc;
            this.response = response ?? Encoding.UTF8.GetBytes(Properties.Resources.app_offline_txt);
        }

        public bool Enabled => enabledFunc();
        public byte[] Response => response;

        public int RetryAfterInSeconds { get; set; } = 3600;
        public string ContentType { get; set; } = "text/html";
    }
}