namespace ODM.UI.WPF.Utils
{
    using System;
    using System.Configuration;
    using System.Net.Http;

    using ODM.UI.WPF.Services;

    public class ClientFactory
    {
        public HttpClient GetClient()
        {
            var clinet = new HttpClient();
            var address = ConfigurationManager.AppSettings[SettingConstants.BaseAddress];
            if (!string.IsNullOrEmpty(address))
                clinet.BaseAddress = new Uri(address);
            return clinet;
        }

    }
}