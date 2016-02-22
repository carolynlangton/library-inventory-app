using System.Windows;
using System.Windows.Controls;

namespace LibrarySystem.AttachedProperties
{
    /// <summary>
    /// The class that defines custom behavior for a textbox control.
    /// </summary>
    public class TextBoxBehavior
    {
        /// <summary>
        /// Creates a dependency property for textboxes to select all the text on focus.
        /// </summary>
        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocus",
                typeof(bool),
                typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        /// <summary>
        /// Gets a value indicating whether or not the select all text property is enabled.
        /// </summary>
        /// <param name="textBox">The textbox control to check.</param>
        /// <returns>A value indicating whether or not the select all text property is enabled.</returns>
        public static bool GetSelectAllTextOnFocus(TextBox textBox)
        {
            return (bool)textBox.GetValue(SelectAllTextOnFocusProperty);
        }

        /// <summary>
        /// Sets a value indicating whether or not the select all text property is enabled.
        /// </summary>
        /// <param name="textBox">The textbox control for which to set the property.</param>
        /// <param name="value">The value to which to set the property.</param>
        public static void SetSelectAllTextOnFocus(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllTextOnFocusProperty, value);
        }

        /// <summary>
        /// Handles the event that the value indicating whether or not the custom property is enabled is changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void OnSelectAllTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                textBox.GotFocus += SelectAll;
                textBox.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                textBox.GotFocus -= SelectAll;
                textBox.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        /// <summary>
        /// Selects all the text in the textbox.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.SelectAll();
        }

        /// <summary>
        /// Ignores the mouse button click if the cursor is already in the textbox.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private static void IgnoreMouseButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null || textBox.IsKeyboardFocusWithin)
            {
                return;
            }

            e.Handled = true;
            textBox.Focus();
        }
    }
}