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
            refresh_button.Visibility = Visibility.Collapsed;
            fetchingRing.IsActive = true;
            fetchingRing.Visibility = Visibility.Visible;
            fetchingText.Visibility = Visibility.Visible;

            var uri = new Uri(MainPage.URL + "scores");

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

            fetchingRing.Visibility = Visibility.Collapsed;
            fetchingRing.IsActive = false;
            fetchingText.Visibility = Visibility.Collapsed;
            refresh_button.Visibility = Visibility.Visible;
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

                string usernamePlusNumber = position.ToString() + "." +  username;
                position++;
                int time_num, time_mins, time_secs;
                int.TryParse(time, out time_num);

                time_mins = time_num / 60;
                time_secs = time_num % 60;

                time = (time_mins.ToString().Length == 1 ? "0" + time_mins.ToString() : time_mins.ToString()) + ":" + (time_secs.ToString().Length == 1 ? "0" + time_secs.ToString() : time_secs.ToString());

                if (position > 11)
                    break;

                ListBoxItem item = new ListBoxItem();

                item.Content = usernamePlusNumber;
                item.FontSize = 20;
                item.FontFamily = new FontFamily("Segoe WP Semibold");
                item.Foreground = new SolidColorBrush(Colors.White);

                scores_name.Items.Add(item);

                item = new ListBoxItem();
                item.Content = value;
                item.FontSize = 20;
                item.FontFamily = new FontFamily("Segoe WP Semibold");
                item.Foreground = new SolidColorBrush(Colors.White);
                item.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

                scores_value.Items.Add(item);

                item = new ListBoxItem();
                item.Content = time;
                item.FontSize = 20;
                item.FontFamily = new FontFamily("Segoe WP Semibold");
                item.Foreground = new SolidColorBrush(Colors.White);
                item.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;

                scores_time.Items.Add(item);

                //Be careful, you take the next from the current item, not from the JArray object.
                jsonArray_Item = jsonArray_Item.Next;
            }
        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            clearListBoxElements();
            getHighScores();
        }

        private void clearListBoxElements()
        {
            while (scores_name.Items.Count != 0)
                for (int i = 0; i < scores_name.Items.Count; i++) {
                    scores_name.Items.RemoveAt(i);
                    scores_value.Items.RemoveAt(i);
                    scores_time.Items.RemoveAt(i);
                }
        }

        private void scores_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int current_index = scores_name.SelectedIndex;
            scores_time.SelectedIndex = current_index;
            scores_value.SelectedIndex = current_index;
        }

        private void scores_value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int current_index = scores_value.SelectedIndex;
            scores_name.SelectedIndex = current_index;
            scores_time.SelectedIndex = current_index;
        }

        private void scores_time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int current_index = scores_time.SelectedIndex;
            scores_name.SelectedIndex = current_index;
            scores_value.SelectedIndex = current_index;
        }
    }
}
