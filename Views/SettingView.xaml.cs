// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for SettingView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODM.UI.WPF.Views
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.ViewModels;

    using ReactiveUI;

    using Splat;

    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingView"/> class.
        /// </summary>
        public SettingView(ISettingService settingService, SettingViewModel settingViewModel)
        {
            this.InitializeComponent();
            this.ViewModel = settingViewModel;
            this.DataContext = this.ViewModel; 

            this.WhenActivated(
                disposable =>
                    {
                        this.BindCommand(this.ViewModel, model => model.SaveSetting, view => view.btnSave)
                            .DisposeWith(disposable);

                        this.ViewModel.SaveSetting.SubscribeOn(RxApp.MainThreadScheduler).Subscribe(
                            b =>
                                {
                                    if (!b) return;
                                    //this.Close();
                                    //var loginView = new LoginView();
                                    // loginView.ShowDialog();
                                }).DisposeWith(disposable);

                        this.ViewModel
                            .SaveSetting
                            .ThrownExceptions
                            .Subscribe(exception =>
                                MessageBox.Show(exception.Message,
                                    "Hata", MessageBoxButton.OK,
                                    MessageBoxImage.Error))
                            .DisposeWith(disposable);
                    });


        }

        public IReadonlyDependencyResolver Resolver => Locator.Current;

        public SettingViewModel ViewModel { get; set; }
    }
}
