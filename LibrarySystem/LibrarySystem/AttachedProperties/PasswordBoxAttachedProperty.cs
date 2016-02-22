using System.Windows;
using System.Windows.Controls;

namespace LibrarySystem.AttachedProperties
{
    /// <summary>
    /// The class that adds an attached property to password boxes.
    /// </summary>
    public static class PasswordBoxAttachedProperty
    {
        /// <summary>
        /// The password box's attached property for the bound password.
        /// </summary>
        public static readonly DependencyProperty BoundPassword =
                  DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxAttachedProperty), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// The password box's attached property for determining whether or not to bind to the password.
        /// </summary>
        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(PasswordBoxAttachedProperty), new PropertyMetadata(false, OnBindPasswordChanged));

        /// <summary>
        /// The password box's attached property for updating the bound password.
        /// </summary>
        public static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAttachedProperty), new PropertyMetadata(false));

        /// <summary>
        /// Sets the value indicating whether or not to bind to the password.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        /// <summary>
        /// Gets the value indicating whether or not to bind to the password.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>The value indicating whether or not to bind to the password.</returns>
        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        /// <summary>
        /// Gets the bound password.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>The bound password.</returns>
        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        /// <summary>
        /// Sets the bound password.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        /// <summary>
        /// Gets the value determining whether the bound password should be updated.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>The value determining whether the bound password should be updated.</returns>
        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        /// <summary>
        /// Sets the value determining whether the bound password should be updated.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }

        /// <summary>
        /// Handles the event of the bound password being changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        /// <summary>
        /// Handles the event of the value determining whether to bind to the password being changed.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event
            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)e.OldValue;
            bool needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= PasswordBoxAttachedProperty.HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += PasswordBoxAttachedProperty.HandlePasswordChanged;
            }
        }

        /// <summary>
        /// Handles the bound password being changed.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            PasswordBoxAttachedProperty.SetUpdatingPassword(box, true);

            // push the new password into the BoundPassword property
            PasswordBoxAttachedProperty.SetBoundPassword(box, box.Password);
            PasswordBoxAttachedProperty.SetUpdatingPassword(box, false);
        }
    }
}