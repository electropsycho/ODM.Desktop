namespace ODM.UI.WPF.Services
{
    using Newtonsoft.Json;

    /// <summary>
    /// The login response.
    /// </summary>
    public class LoginResponse
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("code")]
        public int? Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}