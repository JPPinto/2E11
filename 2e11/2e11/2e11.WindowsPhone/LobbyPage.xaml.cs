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
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LobbyPage : Page
    {

        private Boolean text_changed = false;
        private static String received_invit_name = "";
        public readonly int NUM_OF_TRYS = 6;
        public static List<KeyValuePair<string, string>> main_user;

        public LobbyPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            text_changed = false;
            received_invit_name = "";
        }

        private async void Find_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!text_changed) return;

            postPlayer(userPlaceHolder.Text, "no", "no", "");

            //return;
            progress.IsActive = true;
            progress.Visibility = Visibility.Visible;
            waiting_text_block.Visibility = Visibility.Visible;
            
            for (int i = 0; i < NUM_OF_TRYS; i++)
            {
                await getConnectedPlayers(userPlaceHolder.Text, true, false);
                if (received_invit_name != "")
                    break;
            }

            if (received_invit_name != "")
            {
                //TODO: MANDAR O INVITE CALLBACK
                
                progress.Visibility = Visibility.Collapsed;
                progress.IsActive = false;
                waiting_text_block.Visibility = Visibility.Collapsed;
            }
            else
            {
                waiting_text_block.Visibility = Visibility.Collapsed;
                searching_text_block.Visibility = Visibility.Visible;
                //TODO: SEARCH PLAYERS
                progress.Visibility = Visibility.Collapsed;
                progress.IsActive = false;
                searching_text_block.Visibility = Visibility.Collapsed;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async Task getConnectedPlayers(String username, Boolean checkInvits, Boolean sendInvit)
        {
            List<List<KeyValuePair<string, string>>> values;
            var uri = new Uri(MainPage.URL + "players");
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

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd(); // do something fun...
                    values = parsePlayers(result);

                    if (checkInvits)
                        checkForInvits(values, username);

                    if (sendInvit)
                        sendInvitation(values);
                    //TODO CHECK IF USERNAME ALREADY EXISTS
                }
            }
            catch
            {
                // Details in ex.Message and ex.HResult.       
            }
        }

        private void sendInvitation(List<List<KeyValuePair<string, string>>> values)
        {
            for (int i = 0; i < values.Capacity; i++)
            {
                if (values[i][0].Value != main_user[0].Value)
                {
                    if (values[i][3].Value == "")
                    {
                        received_invit_name = values[i][0].Value;
                    }
                }
            }

            //CHANGE THE INVITED USERS ATTRIBUIT "playAgainstMe"

        }

        private void checkForInvits(List<List<KeyValuePair<string, string>>> values, String username)
        {
            for (int i = 0; i < values.Capacity; i++)
            {
                if (values[i][0].Value == username)
                {
                    if (values[i][3].Value != "")
                    {
                        received_invit_name = values[i][3].Value;
                    }
                }
            }
        }

        private List<List<KeyValuePair<string, string>>> parsePlayers(String jsonArrayAsString)
        {
            var values = new List<List<KeyValuePair<string, string>>>();

            JArray jsonArray = JArray.Parse(jsonArrayAsString);
            JToken jsonArray_Item = jsonArray.First;
            while (jsonArray_Item != null)
            {
                string username = jsonArray_Item.Value<string>("username");
                string hWon = jsonArray_Item.Value<string>("hasWon");
                string hLost = jsonArray_Item.Value<string>("hasLost");
                string pMe = jsonArray_Item.Value<string>("playAgainstMe");

                values.Add(new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("hasWon", hWon),
                    new KeyValuePair<string, string>("hasLost", hLost),
                    new KeyValuePair<string, string>("playAgainstMe", pMe) 
                });

                //Be careful, you take the next from the current item, not from the JArray object.
                jsonArray_Item = jsonArray_Item.Next;
            }
            return values;
        }

        private async void postPlayer(String user, String w, String l, String pMe)
        {
            var uri = new Uri(MainPage.URL + "players");
            var httpClient = new HttpClient(new HttpClientHandler());

            var values = new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>("username", user),
                    new KeyValuePair<string, string>("won", w),
                    new KeyValuePair<string, string>("lost", l),
                    new KeyValuePair<string, string>("playme", pMe) 
              };

            main_user = values; //Stored Current user's data

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            text_changed = true;
        }
    }
}
