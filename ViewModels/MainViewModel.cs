// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MainViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODM.UI.WPF.ViewModels
{
    using System;
    using System.Reactive;

    using DynamicData.Binding;

    using ODM.UI.WPF.Utils;

    using ReactiveUI;

    using Splat;

    /// <summary>
    /// The main view model.
    /// </summary>
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <summary>
        /// The state.
        /// </summary>
        private string state;

        /// <summary>
        /// The is sync.
        /// </summary>
        private bool isSync;


        internal const string OpenedState = "MAİL SENK. AÇIK";
        internal const string ClosedState = "MAİL SENK. KAPALI";

        public MainViewModel(MailSynchronizer mailSynchronizer)
        {
            this.State = ClosedState;

            this.WhenChanged(model => model.IsSync, (model, b) => b)
                .Subscribe(b => { this.State = b ? OpenedState : ClosedState; });

            this.MailSynchronizer = mailSynchronizer;

            this.MailSynchronizer.WhenChanged(synchronizer => synchronizer.IsStarted, (synchronizer, b) => b)
                .Subscribe(b => this.IsSync = b);


            this.StartSyncMailCommand = ReactiveCommand.Create(
                () =>
                    {
                        this.IsSync = true;
                        this.MailSynchronizer.Start();
                        // Console.WriteLine("Zamanlayıcı başladı");
                    }, this.WhenAnyValue(model => model.IsSync, b => !b));

            this.StopSyncMailCommand = ReactiveCommand.Create(
                () =>
                    {
                        this.IsSync = false;
                        this.MailSynchronizer.Stop();
                        // Console.WriteLine("Zamanlayıcı durdu");
                    }, this.WhenAnyValue(model => model.IsSync, b => !!b));


            this.ShowSetting = ReactiveCommand.Create(() => HostScreen.Router
                .Navigate.Execute(Locator.Current.GetService<SettingViewModel>()));
            this.ShowLogin = ReactiveCommand.Create(() => HostScreen.Router
                .Navigate.Execute(Locator.Current.GetService<LoginViewModel>()));
            //this.TestLogin = ReactiveCommand.CreateFromTask(
            //    async (cancellationToken) =>
            //        {
            //            var token = ConfigurationManager.AppSettings[SettingConstants.Token];
            //            if (!string.IsNullOrEmpty(token))
            //            {
            //                using (var client = new HttpClient
            //                                        {
            //                                            BaseAddress = new Uri(
            //                                                ConfigurationManager.AppSettings[SettingConstants
            //                                                    .BaseAddress])
            //                                        })
            //                {
            //                    var response = await client.GetAsync("/api/users/current/permissions");
            //                    if (!response.IsSuccessStatusCode)
            //                    {
            //                        await this.ShowLogin.Execute();
            //                    }
            //                    else
            //                    {
            //                        // this.Confirm("Deneme", "dfgfdgdf");
            //                        //TODO Burada kullanıcıya geri dönüş verilecek
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var res = await this.ShowLogin.Execute();
            //            }
            //        });
        }


        private MailSynchronizer MailSynchronizer { get; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string State
        {
            get => this.state;
            set => this.RaiseAndSetIfChanged(ref this.state, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether ıs sync.
        /// </summary>
        public bool IsSync
        {
            get => this.isSync;
            set => this.RaiseAndSetIfChanged(ref this.isSync, value);
        }

        /// <summary>
        /// Gets the sync mail command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartSyncMailCommand { get; }
        public ReactiveCommand<Unit, Unit> StopSyncMailCommand { get; }

        public ReactiveCommand<Unit, IObservable<IRoutableViewModel>> ShowSetting { get; set; }
        public ReactiveCommand<Unit, IObservable<IRoutableViewModel>> ShowLogin { get; set; }
        public ReactiveCommand<Unit, Unit> TestLogin { get; set; }

        public string UrlPathSegment => UrlConstants.Main;

        public IScreen HostScreen => Locator.Current.GetService<IScreen>();
    }
}
