using System.Windows;
using System.Windows.Controls;

namespace LibrarySystem.AttachedProperties
{
    /// <summary>
    /// The class that defines custom behavior for a password box control.
    /// </summary>
    public class PasswordBoxBehavior
    {
        /// <summary>
        /// Creates a dependency property for password boxes to select all the text on focus.
        /// </summary>
        public static readonly DependencyProperty SelectAllPasswordTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllPasswordTextOnFocus",
                typeof(bool),
                typeof(PasswordBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllPasswordTextOnFocusChanged));

        /// <summary>
        /// Gets a value indicating whether or not the select all text property is enabled.
        /// </summary>
        /// <param name="passwordBox">The password box control to check.</param>
        /// <returns>A value indicating whether or not the select all text property is enabled.</returns>
        public static bool GetSelectAllPasswordTextOnFocus(PasswordBox passwordBox)
        {
            return (bool)passwordBox.GetValue(SelectAllPasswordTextOnFocusProperty);
        }

        /// <summary>
        /// Sets a value indicating whether or not the select all text property is enabled.
        /// </summary>
        /// <param name="passwordBox">The password box control for which to set the property.</param>
        /// <param name="value">The value to which to set the property.</param>
        public static void SetSelectAllPasswordTextOnFocus(PasswordBox passwordBox, bool value)
        {
            passwordBox.SetValue(SelectAllPasswordTextOnFocusProperty, value);
        }

        /// <summary>
        /// Handles the event that the value indicating whether or not the custom property is enabled is changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void OnSelectAllPasswordTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.GotFocus += SelectAll;
                passwordBox.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                passwordBox.GotFocus -= SelectAll;
                passwordBox.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        /// <summary>
        /// Selects all the text in the password box.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var passwordBox = e.OriginalSource as PasswordBox;

            if (passwordBox == null)
            {
                return;
            }

            passwordBox.SelectAll();
        }

        /// <summary>
        /// Ignores the mouse button click if the cursor is already in the password box.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void IgnoreMouseButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null || passwordBox.IsKeyboardFocusWithin)
            {
                return;
            }

            e.Handled = true;
            passwordBox.Focus();
        }
    }
}