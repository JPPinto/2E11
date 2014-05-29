using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using Facebook;
using System.Dynamic;

namespace _2e11
{
    public sealed partial class FacebookLoginPage : Page
    {
        private Facebook.FacebookClient facebook = new Facebook.FacebookClient();

        public FacebookLoginPage()
        {
            this.InitializeComponent();
        }

        private void webView_Loaded(object sender, RoutedEventArgs e) {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = FacebookSettings.AppID;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";
            parameters.response_type = "token";
            parameters.display = "popup";
            parameters.scope = FacebookSettings.ExtendedPermissions;

            var loginUrl = facebook.GetLoginUrl(parameters);

            webView.Navigate(loginUrl);
        }

        private void webView_Navigated(object sender, NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!facebook.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                // user cancelled
                MessageDialogHelper.Show(oauthResult.ErrorDescription, oauthResult.GetType().ToString());
            }
        }

        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    //Dispatcher.BeginInvoke(() => MessageDialogHelper.Show(e.Error.Message, e.GetType().ToString()));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var id = (string)result["id"];

                var url = string.Format("/Pages/FacebookInfoPage.xaml?access_token={0}&id={1}", accessToken, id);

                //Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri(url, UriKind.Relative)));
            };

            fb.GetAsync("me?fields=id");
        }
    }
}