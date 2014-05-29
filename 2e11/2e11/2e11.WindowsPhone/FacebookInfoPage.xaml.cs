using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Facebook;
using System.Collections.Generic;

namespace _2e11 {
    public sealed partial class FacebookInfoPage : Page {
        private string _accessToken;
        private string _userId;

        public FacebookInfoPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            _accessToken = NavigationContext.QueryString["access_token"];
            _userId = NavigationContext.QueryString["id"];
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) {
            LoadFacebookData();
        }

        private void LoadFacebookData() {
            GetUserProfilePicture();

            GraphApiSample();

            FqlSample();
        }

        private void GetUserProfilePicture() {
            // available picture types: square (50x50), small (50xvariable height), large (about 200x variable height) (all size in pixels)
            // for more info visit http://developers.facebook.com/docs/reference/api
            string profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", _userId, "square", _accessToken);

            picProfile.Source = new BitmapImage(new Uri(profilePictureUrl));
        }

        private void GraphApiSample() {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, e) => {
                if (e.Error != null) {
                    //Dispatcher.BeginInvoke(() => MessageDialogHelper.Show(e.Error.Message, e.GetType().ToString()));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                /*
                Dispatcher.BeginInvoke(() =>
                {
                    ProfileName.Text = "Hi " + (string)result["name"];
                    FirstName.Text = "First Name: " + (string)result["first_name"];
                    FirstName.Text = "Last Name: " + (string)result["last_name"];
                });*/
            };

            fb.GetAsync("me");
        }

        private void FqlSample() {
            var fb = new FacebookClient(_accessToken);

            fb.GetCompleted += (o, e) => {
                if (e.Error != null) {
                    //Dispatcher.BeginInvoke(() => MessageDialogHelper.Show(e.Error.Message, e.GetType().ToString()));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var data = (IList<object>)result["data"];

                var count = data.Count;

            };
        }

        private string _lastMessageId;
        private void PostToWall_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(txtMessage.Text)) {
                MessageDialogHelper.Show("Enter message", "");
                return;
            }

            var fb = new FacebookClient(_accessToken);

            fb.PostCompleted += (o, args) => {
                if (args.Error != null) {
                    //Dispatcher.BeginInvoke(() => MessageDialogHelper.Show(args.Error.Message, args.GetType().ToString()));
                    return;
                }

                var result = (IDictionary<string, object>)args.GetResultData();
                _lastMessageId = (string)result["id"];
                /*
                Dispatcher.BeginInvoke(() =>
                {
                    MessageDialogHelper.Show("Message Posted successfully", "");

                    txtMessage.Text = string.Empty;
                    btnDeleteLastMessage.IsEnabled = true;
                });*/
            };

            var parameters = new Dictionary<string, object>();
            parameters["message"] = txtMessage.Text;

            fb.PostAsync("me/feed", parameters);
        }
    }
}
