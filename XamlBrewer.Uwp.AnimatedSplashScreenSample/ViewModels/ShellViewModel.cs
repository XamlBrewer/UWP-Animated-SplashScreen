using Windows.UI.Xaml.Controls;
using XamlBrewer.Uwp.AnimatedSplashScreenSample;

namespace Mvvm
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menu
            // Symbol enumeration is here: https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.symbol.aspx
            Menu.Add(new MenuItem() { Glyph = Symbol.Home, Text = "Home", NavigationDestination = typeof(MainPage) });
            Menu.Add(new MenuItem() { Glyph = Symbol.Street, Text = "Other", NavigationDestination = typeof(OtherPage) });
        }
    }
}
