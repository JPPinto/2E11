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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

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

        private async void Button_Click(object sender, RoutedEventArgs e) 
        {
            var uri = new Uri("http://nameless-meadow-9441.herokuapp.com/allScores");
            var request = new HttpRequestMessage();
            request.RequestUri = uri;
            HttpClient httpClient = new HttpClient();

            // Always catch network exceptions for async methods.
            try 
            {
                var result = await httpClient.GetStringAsync(uri);
                int x = 0;
            }
            catch
            {
                Back.Visibility = Visibility.Collapsed;
                // Details in ex.Message and ex.HResult.       
            }

            // Once your app is done using the HttpClient object call dispose to 
            // free up system resources (the underlying socket and memory used for the object)
            httpClient.Dispose();


            //CreateHttpClient(ref httpClient);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        internal static void CreateHttpClient(ref HttpClient httpClient)
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }

            // Extend HttpClient by chaining filters together
            // and then providing HttpClient with the configured filter pipeline.
            var basefilter = new HttpBaseProtocolFilter();

            // Adds a custom header to every request and response message.
            var myfilter = new PlugInFilter(basefilter);
            httpClient = new HttpClient(myfilter);

        }
    }
}
