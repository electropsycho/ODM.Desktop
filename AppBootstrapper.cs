namespace ODM.UI.WPF
{
    using System;
    using System.Configuration;
    using System.Net.Http;

    //using Autofac;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Services;
    using ODM.UI.WPF.Utils;
    using ODM.UI.WPF.ViewModels;
    using ODM.UI.WPF.Views;

    using ReactiveUI;

    using Serilog;

    using Splat;
    //using Splat.Autofac;

    public class AppBootstrapper
    {
        
        //private static ContainerBuilder container;

        public void SetDependencies()
        {
            //container = new ContainerBuilder();
            //container.RegisterType<LoginView>().AsSelf();
            //container.RegisterType<LoginViewModel>()
            //    .SingleInstance()
            //    .AsSelf();

            //container.RegisterType<MainView>().AsSelf();
            //container.RegisterType<MainViewModel>()
            //    .SingleInstance()
            //    .AsSelf();

            //container.RegisterType<SettingView>().AsSelf();
            //container.RegisterType<SettingViewModel>()
            //    .SingleInstance()
            //    .AsSelf();

            //container.RegisterType<ShellView>()
            //    .SingleInstance()
            //    .AsSelf();
            
            //container.RegisterType<ShellViewModel>()
            //    .SingleInstance()
            //    .As<IScreen>();

            //container.RegisterType<MailSynchronizer>()
            //    .SingleInstance().AsSelf();

            //container.RegisterType<SettingService>()
            //    .SingleInstance()
            //    .As<ISettingService>();

            //container.RegisterType<LoginService>()
            //    .SingleInstance()
            //    .As<ILoginService>();

            //container.RegisterType<ClientFactory>().AsSelf();

            //container.Register(c => 
            //        new LoggerConfiguration()
            //            .WriteTo.Console()
            //            .WriteTo.File("log.txt")
            //            .CreateLogger())
            //    .SingleInstance()
            //    .As<ILogger>();

            //container.UseAutofacDependencyResolver();

            //Locator.CurrentMutable.RegisterLazySingleton(() => new ShellView(), typeof(IViewFor<ShellViewModel>));
           // Locator.CurrentMutable.RegisterLazySingleton(() => new SettingView(), typeof(IViewFor<SettingViewModel>));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new MainView(), typeof(IViewFor<MainViewModel>));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new MailSynchronizer());
        }
    }
}