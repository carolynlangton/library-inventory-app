using System.Windows;

namespace LibrarySystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Contains logic that occurs on the startup of the application.
        /// </summary>
        /// <param name="e">The event's arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create a control window and set its data context
            ControlWindow controlWindow = new ControlWindow();
            controlWindow.DataContext = new ControlWindowViewModel();

            // Set the window's content and content's data context
            (controlWindow.DataContext as ControlWindowViewModel).ControlWindowContent = new LoginView();
            (controlWindow.DataContext as ControlWindowViewModel).ControlWindowContent.DataContext = new LoginViewModel(controlWindow.DataContext as ControlWindowViewModel);

            // Show the window
            controlWindow.Show();
        }
    }
}