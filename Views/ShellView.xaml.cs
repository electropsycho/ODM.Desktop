namespace ODM.UI.WPF.Views
{
    using System.Reactive.Disposables;
    using ReactiveUI;

    using Splat;

    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IViewFor<ShellViewModel>
    {
        public ShellView()
        {
            this.InitializeComponent();
            this.ViewModel = Locator.Current.GetService<ShellViewModel>();
            this.DataContext = this.ViewModel;
            //this.RoutedViewHost.ViewLocator = ViewLocator.Current;
           // this.RoutedViewHost.ViewLocator = viewLocator;
            this.WhenActivated(disposables =>
                {
                    // Bind the view model router to RoutedViewHost.Router property.
                    this.OneWayBind(this.ViewModel, x => x.Router, 
                            x => x.RoutedViewHost.Router)
                        .DisposeWith(disposables);
                });
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (ShellViewModel)value;
        }

        public ShellViewModel ViewModel { get; set; }
    }
}
