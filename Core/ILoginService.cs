namespace ODM.UI.WPF.Core
{
    using System.Threading.Tasks;

    /// <summary>
    /// Oturum işlerini soyutlayan arayüz
    /// </summary>
    public interface ILoginService
    {
        Task<bool> IsLoginRequired();

        Task<(bool, string)> LoginAsync(string email, string password);
    }
}