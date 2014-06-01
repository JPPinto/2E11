using _2e11.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using Newtonsoft.Json.Linq;
using Windows.UI;
using System.Net.Http;
using System.Net;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HighScoresPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public HighScoresPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            getHighScores();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            clearListBoxElements();
        }

        #endregion

        private async void getHighScores()
        {
            var uri = new Uri(MainPage.URL + "scores");
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = uri;
            var httpClient = new HttpClient(new HttpClientHandler());

            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            if (request.Headers == null)
                request.Headers = new WebHeaderCollection();
            request.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.UtcNow.ToString();
            
            // Always catch network exceptions for async methods.
            try
            {
                WebResponse response = await request.GetResponseAsync();
                
                //response.EnsureSuccessStatusCode();
                //var responseString = await response.Content.ReadAsStringAsync();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd(); // do something fun...
                    ParseScores(result);
                }
            }
            catch
            {
                // Details in ex.Message and ex.HResult.       
            }
        }

        private async void getConnectedPlayers()
        {
            /*var uri = new Uri(MainPage.URL + "players");
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = uri;
            var httpClient = new HttpClient(new HttpClientHandler());

            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            if (request.Headers == null)
                request.Headers = new WebHeaderCollection();
            request.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.UtcNow.ToString();

            // Always catch network exceptions for async methods.
            try
            {
                WebResponse response = await request.GetResponseAsync();

                //response.EnsureSuccessStatusCode();
                //var responseString = await response.Content.ReadAsStringAsync();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd(); // do something fun...
                    ParseScores(result);
                }
            }
            catch
            {
                // Details in ex.Message and ex.HResult.       
            }*/
        }

        private async void postPlayer(String user, String w, String l)
        {
            var uri = new Uri(MainPage.URL + "addConnectPlayer");
            var httpClient = new HttpClient(new HttpClientHandler());

            var values = new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>("username", user),
                    new KeyValuePair<string, string>("hasWon", w),
                    new KeyValuePair<string, string>("hasLost", l) 
              };

            // Always catch network exceptions for async methods.
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(uri, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                if (responseString.Equals("Added")) return;

            }
            catch
            {
                // Details in ex.Message and ex.HResult.       
            }

        }

        public void ParseScores(string jsonArrayAsString)
        {
            int position = 1;

            JArray jsonArray = JArray.Parse(jsonArrayAsString);
            JToken jsonArray_Item = jsonArray.First;
            while (jsonArray_Item != null)
            {
                string username = jsonArray_Item.Value<string>("username");
                string value = jsonArray_Item.Value<string>("value");
                string time = jsonArray_Item.Value<string>("time");

                string totalString = position.ToString() + ".  " +  username + "\t     " + value + "\t\t" + time;
                position++;

                ListBoxItem item = new ListBoxItem();

                item.Content = totalString;
                item.FontSize = 20;
                item.FontFamily = new FontFamily("Segoe WP Semibold");
                item.Foreground = new SolidColorBrush(Colors.White);

                scores.Items.Add(item);

                //Be careful, you take the next from the current item, not from the JArray object.
                jsonArray_Item = jsonArray_Item.Next;
            }
        }

        private void About_Button_Click(object sender, RoutedEventArgs e)
        {
            clearListBoxElements();
            getHighScores();
        }

        private void clearListBoxElements()
        {
            while (scores.Items.Count != 0)
                for (int i = 0; i < scores.Items.Count; i++)
                    scores.Items.RemoveAt(i);
        }
    }
}
