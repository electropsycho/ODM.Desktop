namespace ODM.UI.WPF.Services
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Ext;
    using ODM.UI.WPF.Utils;

    using ILogger = Serilog.ILogger;

    /// <summary>
    /// Tüm kullanıcı giriş ve jeton işlerini halleden gerçek sınıf
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly ISettingService settingService;

        private readonly ClientFactory clientFactory;

        private ILogger logger;

        public LoginService(ILogger logger, 
                            ISettingService settingService, 
                            ClientFactory clientFactory)
        {
            this.settingService = settingService;
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        public async Task<bool> IsLoginRequired()
        {
            try
            {
                var client = clientFactory.GetClient();
                if (!this.settingService.HasToken())
                {
                    return true;
                }
#if DEBUG
                var response = await client.GetAsync("/api/units");
#endif
#if !DEBUG
                var response = await this.client.GetAsync("/otomasyon/api/units");
#endif
                return !response.IsSuccessStatusCode;
            }
            catch (Exception exception)
            {
                this.logger.Error(exception, "Jeton deneme hatası");
                return true;
            }
        }

        public async Task<(bool, string)> LoginAsync(string email, string password)
        {
            var data = new { email, password };
            const string unsuccessfulLogin = "Oturum açma başarısız oldu!";

            try
            {
                var client = this.clientFactory.GetClient();

#if DEBUG
                var response = await client.PostAsync("/api/auth/login", data.AsJson());
#endif
#if !DEBUG
                var response = await client.PostAsync("/otomasyon/api/auth/login", data.AsJson());
#endif
                if (!response.IsSuccessStatusCode) return (false, unsuccessfulLogin);
                var result = await response.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<LoginResponse>(result);
                if (res.Code != null && res.Code == 1001)
                {
                    return (WasLoggedIn: false, res.Message);
                }

                this.settingService.Write(SettingConstants.Token, res.Token);
                return (WasLoggedIn: true, Message: "Oturum açma başarılı oldu.");
            }
            catch (Exception exception)
            {
                this.logger.Error(exception, "Kullanıcı giriş hatası");
                return (WasLoggedIn: false, Message: unsuccessfulLogin);
            }

        }
    }
}