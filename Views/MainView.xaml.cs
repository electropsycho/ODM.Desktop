// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODM.UI.WPF.Views
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows;
    using System.Windows.Media;

    using ODM.UI.WPF.ViewModels;

    using ReactiveUI;

    using Brush = System.Windows.Media.Brush;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView(MainViewModel mainViewModel)
        {
            this.InitializeComponent();
            this.ViewModel = mainViewModel;

            this.WhenActivated(
                (disposable) =>
                    {
                        //this.OneWayBind(this.ViewModel, model => model.State, window => window.btnState.Content)
                        //    .DisposeWith(disposable);

                        //this.OneWayBind(
                        //    this.ViewModel,
                        //    model => model.IsSync,
                        //    window => window.btnState.Background,
                        //    this.BoolToBackroundConverterFunc).DisposeWith(disposable);

                        //this.OneWayBind(
                        //    this.ViewModel,
                        //    model => model.IsSync,
                        //    window => window.btnState.Foreground,
                        //    this.BoolToForegroundConverterFunc)
                        //    .DisposeWith(disposable);

                        this.BindCommand(this.ViewModel, model => model.StopSyncMailCommand, window => window.btnStop)
                            .DisposeWith(disposable);

                        this.BindCommand(this.ViewModel, model => model.StartSyncMailCommand, window => window.btnStart)
                            .DisposeWith(disposable);

                        this.BindCommand(this.ViewModel, model => model.ShowSetting, window => window.btnShowSetting)
                            .DisposeWith(disposable);

                        this.BindCommand(this.ViewModel, model => model.ShowLogin, window => window.btnShowLogin)
                            .DisposeWith(disposable);

                        this.ViewModel.ShowSetting.ThrownExceptions.Subscribe(
                            exception => Console.WriteLine(exception.Message));
                        this.ViewModel.ShowSetting.Subscribe(b => {});
                    });
        }

        /// <summary>
        /// The bool to backround converter func.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns> 
        private Brush BoolToBackroundConverterFunc(bool value)
        {
            return value ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
        }

        /// <summary>
        /// The bool to foreground converter func.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns>
        private Brush BoolToForegroundConverterFunc(bool value)
        {
            return value ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        }


        /// <summary>
        /// The view model property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty;

        /// <summary>
        /// Initializes static members of the <see cref="MainView"/> class.
        /// </summary>
        static MainView()
        {
            ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainView));
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public MainViewModel ViewModel
        {
            get => (MainViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
    }
}
