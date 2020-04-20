// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The setting view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODM.UI.WPF.ViewModels
{
    using System;
    using System.Reactive;
    using System.Text.RegularExpressions;

    using ODM.UI.WPF.Core;
    using ODM.UI.WPF.Services;
    using ODM.UI.WPF.Utils;

    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using ReactiveUI.Validation.Extensions;
    using ReactiveUI.Validation.Helpers;

    using Serilog;

    /// <summary>
    /// The setting view model.
    /// </summary>
    public class SettingViewModel :
        ReactiveValidationObject<SettingViewModel>, IRoutableViewModel
    {
        private readonly IScreen screen;

        public ReactiveCommand<Unit, bool> SaveSetting { get; }

        //private static readonly Regex numbeRegex = new Regex(@"^\d+$");
        private static readonly Regex urlRegex = new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$");

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingViewModel"/> class.
        /// </summary>
        public SettingViewModel(ISettingService settingService, 
                                IScreen screen, ILogger logger)
        {
            this.screen = screen;

            var service = settingService;
            // var synchronizer = Locator.Current.GetService<MailSynchronizer>();
            this.BaseAddress = service.Get(SettingConstants.BaseAddress);
            this.MailSyncInterval = service.Get(SettingConstants.MailSyncInterval);

            this.SetValidations();

            var canExecute = this.WhenAnyValue(
                x => x.BaseAddress,
                x => x.MailSyncInterval,
                (baseAddress, interval) => 
                    !this.HasErrors);


            this.SaveSetting = ReactiveCommand.Create(
                () =>
                    {
                        service.Write(SettingConstants.BaseAddress, this.BaseAddress);
                        service.Write(SettingConstants.MailSyncInterval, this.MailSyncInterval);
                        return true;
                    },
                canExecute);
            
        }

        private void SetValidations()
        {
            this.ValidationRule(
                model => model.MailSyncInterval,
                s =>
                    {
                        var r1 = !string.IsNullOrEmpty(s);
                        var r2 = double.TryParse(s, out var result);
                        var r3 = result > 0;
                        return r1 && r2 && r3;
                    },
                "Lütfen sayı değeri giriniz");
            this.ValidationRule(
                model => model.BaseAddress,
                s => !string.IsNullOrEmpty(s) && urlRegex.IsMatch(s),
                "Lütfen geçerli bir adres giriniz");
        }

        [Reactive]
        public string MailSyncInterval { get; set; }

        [Reactive]
        public string BaseAddress { get; set; }

        public string UrlPathSegment => UrlConstants.Setting;

        public IScreen HostScreen => this.screen;
    }
}
