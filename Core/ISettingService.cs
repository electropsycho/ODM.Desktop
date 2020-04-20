namespace ODM.UI.WPF.Core
{
    public interface ISettingService
    {
        bool HasValue(string key);
       
        bool HasKey(string key);

        void Write(string key, string value);

        string Get(string key);

        bool HasSettings();

        bool HasToken();
    }
}