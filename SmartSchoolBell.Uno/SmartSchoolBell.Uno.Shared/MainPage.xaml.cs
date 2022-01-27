using Windows.UI.Xaml.Controls;
using SmartSchoolBell.Uno.Views;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;

namespace SmartSchoolBell.Uno
{
    public sealed partial class MainPage : Page
    {
        public MainPage() => this.InitializeComponent();

        private void MainNavigationView_OnItemInvoked(NavigationView navigationView, NavigationViewItemInvokedEventArgs navigationViewItemInvokedEventArgs)
        {
            if (navigationViewItemInvokedEventArgs.InvokedItemContainer.Tag.ToString() == "SamplePage1")
                MainFrame.Navigate(typeof(TimetablePage));
        }
    }
}