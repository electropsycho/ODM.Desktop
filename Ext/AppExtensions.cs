// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODM.UI.WPF.Ext
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Newtonsoft.Json;

    using ReactiveUI;

    using Splat;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class AppExtensions
    {
        /// <summary>
        /// The get view.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Window"/>.
        /// </returns>
        /// <exception cref="TypeAccessException">
        /// </exception>
        public static Window GetView<T>(this T viewModel) where T : ReactiveObject
        {
            var view = Locator.Current.GetService<IViewFor<T>>();
            // view.ViewModel = viewModel;
            var window = view as Window;
            if (window == null)
                throw new TypeAccessException("View not implement IViewFor");
            return window;
        }

        /// <summary>
        /// The as json.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="StringContent"/>.
        /// </returns>
        public static StringContent AsJson(this object o)
        {
            return new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
        }
    }

    public static class Secure
    {
        private static readonly DependencyProperty PasswordInitializedProperty =
            DependencyProperty.RegisterAttached("PasswordInitialized", typeof(bool), typeof(Secure), new PropertyMetadata(false));

        private static readonly DependencyProperty SettingPasswordProperty =
            DependencyProperty.RegisterAttached("SettingPassword", typeof(bool), typeof(Secure), new PropertyMetadata(false));

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }
        // We play a trick here. If we set the initial value to something, it'll be set to something else when the binding kicks in,
        // and HandleBoundPasswordChanged will be called, which allows us to set up our event subscription.
        // If the binding sets us to a value which we already are, then this doesn't happen. Therefore start with a value that's
        // definitely unique.
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(Secure),
                new FrameworkPropertyMetadata(Guid.NewGuid().ToString(), HandleBoundPasswordChanged)
                {
                    BindsTwoWayByDefault = true,
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus // Match the default on Binding
                });

        private static void HandleBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = dp as PasswordBox;
            if (passwordBox == null)
                return;

            // If we're being called because we set the value of the property we're bound to (from inside 
            // HandlePasswordChanged, then do nothing - we already have the latest value).
            if ((bool)passwordBox.GetValue(SettingPasswordProperty))
                return;

            // If this is the initial set (see the comment on PasswordProperty), set ourselves up
            if (!(bool)passwordBox.GetValue(PasswordInitializedProperty))
            {
                passwordBox.SetValue(PasswordInitializedProperty, true);
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }

            passwordBox.Password = e.NewValue as string;
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            passwordBox.SetValue(SettingPasswordProperty, true);
            SetPassword(passwordBox, passwordBox.Password);
            passwordBox.SetValue(SettingPasswordProperty, false);
        }
    }
}