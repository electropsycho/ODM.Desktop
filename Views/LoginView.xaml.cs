namespace ODM.UI.WPF.Views
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows;
    using System.Windows.Controls;

    using ODM.UI.WPF.ViewModels;

    using ReactiveUI;

    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : ReactiveUserControl<LoginViewModel>
    {
        public LoginView(LoginViewModel loginViewModel)
        {
            this.InitializeComponent();
            //var confirm = (Func<string, string, bool>)((msg, capt) => MessageBox.Show(msg, capt, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
             this.ViewModel = loginViewModel;
            this.DataContext = this.ViewModel;
            this.WhenActivated(
                disposable =>
                    {
                        this.BindCommand(this.ViewModel, model => model.LoginCommand, view => view.btnLogin)
                            .DisposeWith(disposable);
                    });
        }

        public LoginViewModel ViewModel { get; }
    }
}
