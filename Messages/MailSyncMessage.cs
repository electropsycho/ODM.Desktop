namespace ODM.UI.WPF.Messages
{
     public class MailSynced
    {
        public ResponseMessage Message { get; }

        public MailSynced(ResponseMessage message)
        {
            this.Message = message;
        }
    }
}
