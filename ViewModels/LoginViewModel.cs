namespace ODM.UI.WPF.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Utils;

    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using ReactiveUI.Validation.Extensions;
    using ReactiveUI.Validation.Helpers;

    using Splat;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel : ReactiveValidationObject<LoginViewModel>, IRoutableViewModel
    {
        /// <summary>
        /// Gets the confirm.
        /// </summary>
        // public Func<string, string, bool> Confirm { get; }


        private bool isLoggedIn;
        private static readonly Regex EmailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                                             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase);

        public LoginViewModel(ILoginService loginService)
        {
            this.LoginService = loginService;

            this.SetValidations();

            var canExecute = this.WhenAnyValue(
                model => model.Email,
                model => model.Password,
                (email, password) => !this.HasErrors);


            this.LoginCommand = ReactiveCommand.CreateFromTask(Login, canExecute);

            this.LoginCommand.SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(tuple =>
                {
                    this.IsLoggedIn = tuple.Item1;
                    this.Message = tuple.Item2;
                });

            // this.LoginCommand.ToProperty(this, model => this.IsLoggedIn, scheduler: RxApp.MainThreadScheduler);
        }

        private void SetValidations()
        {
            this.ValidationRule(
                model => model.Email,
                s => !string.IsNullOrEmpty(s) && EmailRegex.IsMatch(s),
                "Lütfen geçerli bir mail adresi giriniz!");
            this.ValidationRule(
                model => model.Password,
                s => !string.IsNullOrEmpty(s) && s.Length >= 6,
                "Lütfen en az 6 karakterden oluşan şifrenizi giriniz!");
        }

        async Task<(bool, string)> Login(CancellationToken token)
        {
            var res = await this.LoginService.LoginAsync(this.Email, this.Password);
            // MessageBus.Current.SendMessage(new StateMessage(res));
            return res;
        }

        public bool IsLoggedIn
        {
            get => this.isLoggedIn;
            set => this.RaiseAndSetIfChanged(ref this.isLoggedIn, value);
        }

        private ILoginService LoginService { get; set; }

        [Reactive]
        public string Message { get; set; }

        [Reactive]
        public string Email { get; set; }

        [Reactive]
        public string Password { get; set; }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public ReactiveCommand<Unit, (bool, string)> LoginCommand { get; }

        public string UrlPathSegment => UrlSegments.Login;

        public IScreen HostScreen => Locator.Current.GetService<IScreen>();

    }
}