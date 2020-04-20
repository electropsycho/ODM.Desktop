namespace ODM.UI.WPF
{
    using System;
    using System.Reactive.Linq;

    using MahApps.Metro.Controls.Dialogs;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Services;
    using ODM.UI.WPF.ViewModels;

    using ReactiveUI;

    public class ShellViewModel : ReactiveObject
    {

        private ISettingService SettingService { get; }

        public ILoginService LoginService { get; }

        public RoutingState Router { get; }

        public SettingViewModel SettingViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
        public MainViewModel MainViewModel{ get; set; }

        public ShellViewModel(IScreen screen,
                              ISettingService settingServices, 
                              ILoginService loginService,
                              IDialogCoordinator coordinator,
                              SettingViewModel settingViewModel,
                              LoginViewModel loginViewModel,
                              MainViewModel mainViewModel)
        {
            this.SettingService = settingServices;
            this.LoginService = loginService;
            this.Router = screen.Router;
            this.SettingViewModel = settingViewModel;
            this.LoginViewModel = loginViewModel;
            this.MainViewModel = mainViewModel;

            //Router.NavigationStack.Add(this.SettingViewModel);
            //Router.NavigationStack.Add(this.LoginViewModel);
            //Router.NavigationStack.Add(this.MainViewModel);

            if (!this.SettingService.HasSettings())
            {
                this.Router.Navigate.Execute(this.SettingViewModel);
            }
            else if(!this.SettingService.HasToken())
            {
                this.Router.Navigate.Execute(this.LoginViewModel);
            }
            else
            {
                this.Router.Navigate.Execute(this.MainViewModel);
            }

            this.SettingViewModel.SaveSetting
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                async o =>
                    {
                        var loginRequired = await loginService.IsLoginRequired();
                        if (o && loginRequired)
                        {
                            this.Router.Navigate.Execute(this.LoginViewModel);
                        }
                        else
                        {
                            this.Router.Navigate.Execute(this.MainViewModel);
                        }
                    });

            this.LoginViewModel.LoginCommand.Subscribe(
                async tuple =>
                    {
                        if (tuple.Item1)
                        {
                            await this.Router.Navigate.Execute(this.MainViewModel);
                        }
                        else
                        {
                            await coordinator.ShowMessageAsync(
                                this,
                                "Hata!",
                                "Maalesef oturum açılamadı. Detaylı bilgi için kurulum klasörü içinde yer alan log.txt dosyasına bakabilirsiz.");
                        }
                    });
        }
    }
}