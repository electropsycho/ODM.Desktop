namespace ODM.UI.WPF.Services
{
    using System.Configuration;

    using ODM.UI.WPF.Core;

    /// <summary>
    /// The setting service.
    /// </summary>
    public class SettingService : ISettingService
    {
        private KeyValueConfigurationCollection settings = null;
        private Configuration configuration;

        public SettingService()
        {
            this.configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.settings = this.configuration.AppSettings.Settings;
        }

        /// <summary>
        /// The check setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasValue(string key)
        {
            ConfigurationManager.RefreshSection(this.configuration.AppSettings.SectionInformation.Name);
            return this.settings[key]?.Value != null;
        }

        public bool HasKey(string key)
        {
            ConfigurationManager.RefreshSection(this.configuration.AppSettings.SectionInformation.Name);
            return this.settings[key] != null;
        }

        /// <summary>
        /// The write setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(string key, string value)
        {
            if (!this.HasValue(key))
            {
                this.settings.Add(key, value);
            }
            else
            {
                this.settings[key].Value = value;
            }
            this.configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(this.configuration.AppSettings.SectionInformation.Name);
        }

        /// <summary>
        /// The get setting.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// The has settings.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasSettings()
        {
            var result = this.HasValue(SettingConstants.BaseAddress) &&
                         this.HasValue(SettingConstants.MailSyncInterval);
            return result;
        }

        /// <summary>
        /// The has token.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasToken()
        {
            var res = this.HasValue(SettingConstants.Token);
            return res;
        }
    }
}