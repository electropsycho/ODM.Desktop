using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using Autofac;
using DynamicData.Binding;
using MahApps.Metro.Controls.Dialogs;
using ODM.UI.WPF.Core;
using ODM.UI.WPF.Messages;
using ODM.UI.WPF.Services;
using ODM.UI.WPF.Utils;
using ODM.UI.WPF.ViewModels;
using ODM.UI.WPF.Views;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Autofac;
using Application = System.Windows.Application;
using ILogger = Serilog.ILogger;
using MessageBox = System.Windows.MessageBox;

namespace ODM.UI.WPF
{
    using static DialogCoordinator;

    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        private static IReadonlyDependencyResolver container;

        private static App app;

        private static NotifyIcon notifyIcon;

        private static bool isExit;

        [STAThread]
        private static void Main()
        {
            SetDependencies();
            RunApplication();
        }

        /// <summary>
        /// The set dependencies.
        /// </summary>
        private static void SetDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(context => Instance)
                .As<IDialogCoordinator>();

            containerBuilder.RegisterType<LoginView>().AsSelf();
            containerBuilder.RegisterType<LoginViewModel>()
                .SingleInstance()
                .AsSelf();

            containerBuilder.RegisterType<MainView>().AsSelf();
            containerBuilder.RegisterType<MainViewModel>()
                .SingleInstance()
                .AsSelf();

            containerBuilder.RegisterType<SettingView>().AsSelf();
            containerBuilder.RegisterType<SettingViewModel>()
                .SingleInstance()
                .AsSelf();

            containerBuilder.RegisterType<ShellView>()
                .SingleInstance()
                .AsSelf();

            containerBuilder.RegisterType<ShellViewModel>()
                .SingleInstance()
                .AsSelf();

            containerBuilder.RegisterType<Routing>()
                .SingleInstance()
                .As<IScreen>();

            containerBuilder.RegisterType<MailSynchronizer>()
                .SingleInstance().AsSelf();

            containerBuilder.RegisterType<SettingService>()
                .SingleInstance()
                .As<ISettingService>();

            containerBuilder.RegisterType<LoginService>()
                .SingleInstance()
                .As<ILoginService>();

            containerBuilder.RegisterType<ClientFactory>().AsSelf();

            containerBuilder.Register(c =>
                    new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File("log.txt")
                        .CreateLogger())
                .SingleInstance()
                .As<ILogger>();

            containerBuilder.UseAutofacDependencyResolver();
            container = Locator.Current;
        }

        /// <summary>
        /// The run application.
        /// </summary>
        private static void RunApplication()
        {
            app = new App();
            try
            {
                var mainWindow = container.GetService<ShellView>();
                app.MainWindow = mainWindow;
                app.MainWindow.Closing += ShellViewClosing;
                app.InitializeComponent();
                Startup();
#if DEBUG
                app.MainWindow.Show();
#endif
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Maalasef ölümcül bir hata ile karşılaştık ayrıntılı bilgi kurulum klasöründe bulunan log.txt dosyasına bakınız!",
                    "Hata!",
                    MessageBoxButton.OK);
                container.GetService<ILogger>()
                    .Fatal(ex, "Ölümcül hata!!");
            }
        }

        /// <summary>
        /// The startup.
        /// </summary>
        private static void Startup()
        {
            SetNotifyIcon();
            MessageBus.Current.Listen<MailSynced>()
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    m =>
                        {
                            switch (m.Message.Code)
                            {
                                case 1000:
                                    notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                                    notifyIcon.BalloonTipTitle = "Bilgi";
                                    notifyIcon.BalloonTipText = m.Message.Text;
                                    notifyIcon.ShowBalloonTip(3000);
                                    break;
                                case 1005:
                                    notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                                    notifyIcon.BalloonTipTitle = "Hata!";
                                    notifyIcon.BalloonTipText = m.Message.Text;
                                    notifyIcon.ShowBalloonTip(3000);
                                    break;
                            }
                        });
        }

        /// <summary>
        /// The set notify icon.
        /// </summary>
        private static void SetNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/geometry.ico"));
            notifyIcon.Icon = new Icon(iconStream?.Stream ?? throw new NullReferenceException());
            notifyIcon.Visible = true;

#if !DEBUG
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = "Bilgi";
            notifyIcon.BalloonTipText = "ODM Mail senkronlayıcı çalışıyor";
            notifyIcon.ShowBalloonTip(3000);
#endif
            CreateContextMenu();
        }

        /// <summary>
        /// The create context menu.
        /// </summary>
        private static void CreateContextMenu()
        {
            var mailSynchronizer = container.GetService<MailSynchronizer>();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            var showButton = notifyIcon.ContextMenuStrip.Items.Add("Göster");
            showButton.Click += (s, e) => ShowMainWindow();
            showButton.Image = Image.FromStream(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/eye.png"))?.Stream);

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
             
            var start = notifyIcon.ContextMenuStrip.Items.Add("Başlat");
            start.Click += (s, e) => { mailSynchronizer.Start(); };
            start.Image = Image.FromStream(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/play-circle.png"))?.Stream);
            
            var stop = notifyIcon.ContextMenuStrip.Items.Add("Durdur");
            stop.Click += (s, e) => mailSynchronizer.Stop();
            stop.Image = Image.FromStream(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/stop-circle.png"))?.Stream);
            
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            var exitButton = notifyIcon.ContextMenuStrip.Items.Add("Çıkış");
            exitButton.Click += (s, e) => ExitApplication();
            exitButton.Image = Image.FromStream(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/exit-to-app.png"))?.Stream);

            mailSynchronizer.WhenChanged(synchronizer => synchronizer.IsStarted,
                (synchronizer, b) => b)
                .Subscribe(b =>
                {
                    if (b)
                    {
                        start.Enabled = false;
                        stop.Enabled = true;
                    }
                    else
                    {
                        start.Enabled = true;
                        stop.Enabled = false;
                    }
                });
        }

        /// <summary>
        /// The exit application.
        /// </summary>
        private static void ExitApplication()
        {
            isExit = true;
            app.MainWindow?.Close();
            notifyIcon.Dispose();
            notifyIcon = null;
            GC.Collect();
        }

        /// <summary>
        /// The show main window.
        /// </summary>
        private static void ShowMainWindow()
        {
            if (app.MainWindow != null && app.MainWindow.IsVisible)
            {
                if (app.MainWindow.WindowState == WindowState.Minimized)
                {
                    app.MainWindow.WindowState = WindowState.Normal;
                }

                app.MainWindow.Activate();
            }
            else
            {
                app.MainWindow?.Show();
            }
        }

        /// <summary>
        /// The shell view closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ShellViewClosing(object sender, CancelEventArgs e)
        {
            if (isExit) return;
            e.Cancel = true;
            app.MainWindow?.Hide(); // A hidden window can be shown again, a closed one not
        }
    }
} 