namespace ODM.UI.WPF.Utils
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Timers;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Messages;
    using ODM.UI.WPF.Services;

    using ReactiveUI;

    using Serilog;

    public class MailSynchronizer : ReactiveObject
    {
        private static bool isStarted;

        private ISettingService settingService;
        private HttpClient client;
        private ILogger logger;

        public MailSynchronizer(ISettingService settingService, ILogger logger, ClientFactory clientFactory)
        {
            this.settingService = settingService;
            this.client = clientFactory.GetClient();
            var res = double.TryParse(settingService.Get(SettingConstants.MailSyncInterval), out var result);
            if (!res) result = 60;
            this.Timer = new Timer
                             {
                                 Interval = 60 * 1000 * result
                             };

            this.logger = logger;
        }

        /// <summary>
        /// The elapsed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private async void ElapsedEvent(object sender, ElapsedEventArgs args)
        {
            Console.WriteLine($@"Saat: {DateTime.Now.ToLongTimeString()}");
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/mails/sync");
            request.Headers.Accept.Clear();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.settingService.Get(SettingConstants.Token));
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settingService.Get(SettingConstants.Token));
            var resp = await client.SendAsync(request);
            try
            {
                //var resObj = JsonConvert.DeserializeObject<ResponseMessage>(content);
                MessageBus.Current.SendMessage(
                    resp.IsSuccessStatusCode
                        ? new MailSynced(new ResponseMessage(1000, "Mail Senk. Başarılı"))
                        : new MailSynced(new ResponseMessage(1005, "Mail Senk. Başarısız Oldu!")));
            }
            catch (Exception exception)
            {
                this.logger.Error(exception.Message);
                MessageBus.Current.SendMessage(new MailSynced(new ResponseMessage(1005, "Mail Senk. Başarısız Oldu!", exception.Message)));
            }
        }


        /// <summary>
        /// Gets or sets the timer.
        /// </summary>
        private Timer Timer { get; }

        public bool IsStarted
        {
            get => isStarted;
            set => this.RaiseAndSetIfChanged(ref isStarted, value);
        }

        public void Start()
        {
            if (this.Timer == null) throw new NullReferenceException();
            this.Timer.Elapsed += ElapsedEvent;
            this.Timer.Start();
            this.IsStarted = true;
        }

        public void Stop()
        {
            this.Timer.Elapsed -= ElapsedEvent;
            this.Timer.Stop();
            this.IsStarted = false;
        }

        public void Restart()
        {
            if (!this.IsStarted) return;
            this.Timer.Stop();
            this.SetInterval(double.Parse(settingService.Get(SettingConstants.MailSyncInterval)));
            this.Timer.Start();
        }

        private void SetInterval(double minute)
        {
            this.Timer.Interval = 60 * 1000 * minute;
        }
    }
}