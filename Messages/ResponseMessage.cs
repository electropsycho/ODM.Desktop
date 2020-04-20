namespace ODM.UI.WPF.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// The response message.
    /// </summary>
    public class ResponseMessage
    {
        [JsonProperty("code")]
        public int Code { get; }

        [JsonProperty("message")]
        public string Text { get; }

        [JsonProperty("exception")]
        public string Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public ResponseMessage(int code, string message, string exception = null)
        {
            this.Code = code;
            this.Text= message;
            this.Exception = exception;
        }

        public ResponseMessage()
        {

        }
    }
}