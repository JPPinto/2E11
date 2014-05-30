using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LobbyPage : Page
    {
        private int timeout;
        public LobbyPage()
        {
            this.InitializeComponent();
            timeout = 30;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            point1.Visibility = Visibility.Visible;
            point2.Visibility = Visibility.Visible;
            point3.Visibility = Visibility.Visible;

            for (int i = 0; i < timeout; i++) {
                point1.IsChecked = true;
                
                point2.IsChecked = true;
                //System.Threading.Thread.Sleep(50);
                point3.IsChecked = true;

            }
        }
    }
}
