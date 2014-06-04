using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.System.Threading;
using System.Net.Http;
using System.Collections.Generic;

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class GamePage : Page
    {

        Game game;
        DispatcherTimer timer;
        ushort tMins, tSecs;

        private double old_X;
        private double old_Y;
        private bool animating;
        private BitmapImage[] tiles;
        private Boolean tapped = false;
        public static Boolean multiplayer = false;
        private static readonly string default_text = "Playing Against:\n";
        public static List<KeyValuePair<string, string>> opponent;

        public GamePage()
        {
            this.InitializeComponent();
            preloadImages();
            //ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            game = new Game();
            animating = false;

            initializeTimer();

            UpdateGrid();
            timer.Start();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tapped = false;
            game.resetBoard();
            game.addStartTiles();

            //Restart Timer
            timer.Start();

            if (multiplayer)
            {
                player_against_text_block.Text = default_text + LobbyPage.received_invit_name;
                player_against_text_block.Visibility = Visibility.Visible;
            }

            UpdateGrid();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(dataTransferManager_DataRequested);

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
            tMins = 0;
            tSecs = 0;

            if (multiplayer)
            {
                LobbyPage.deletePlayer(LobbyPage.main_user[0].Value);
                timer.Stop();
                showSubmitMenu("You lost...");
            }

            player_against_text_block.Visibility = Visibility.Collapsed;
            multiplayer = false;
            player_against_text_block.Text = default_text;
            base.OnNavigatedFrom(e);
        }

        private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = "My HighScore In 2E11";
            requestData.Properties.Description = "My HighScore In 2E11";
            requestData.SetText("I got " + game.score / 2 + " points in 2E11! How much can you get?");
        }

        private void Image_Left_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.moveLeft();
            UpdateGrid();
        }

        private void Image_Right_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.moveRight();
            UpdateGrid();
        }

        private void Image_Top_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.moveUp();
            UpdateGrid();
        }

        private void Image_Bottom_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.moveDown();
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            gameGrid.Children.Clear();

            ushort[,] temp_board = game.getBoard();

            for (ushort i = 0; i < Game.boardSize; i++)
            {
                for (ushort j = 0; j < Game.boardSize; j++)
                {
                    if (temp_board[i, j] != 0)
                    {
                        int tile_value = temp_board[i, j];
                        if (tile_value != 0)
                            addPiece(temp_board[i, j], j, i);
                    }

                }
            }

            this.score_value.Text = (game.getScore() / 2).ToString();

            if (game.isLost)
            {
                if (multiplayer)
                    LobbyPage.playerLost(LobbyPage.main_user[0].Value, true);

                timer.Stop();
                LobbyPage.received_invit_name = "";
                showSubmitMenu("You Lose...");
            }

            if (game.isWon)
            {
                if (multiplayer)
                    LobbyPage.playerWon(LobbyPage.main_user[0].Value, true);
                timer.Stop();
                LobbyPage.received_invit_name = "";
                showSubmitMenu("You Win!");
            }
        }

        private void addPiece(ushort value, int x_pos, int y_pos)
        {
            Image temp_img = new Image();
            temp_img.Source = tiles[value - 1];

            Grid.SetColumn(temp_img, x_pos);
            Grid.SetRow(temp_img, y_pos);

            gameGrid.Children.Add(temp_img);

        }

        private void preloadImages()
        {
            tiles = new BitmapImage[11];

            for (int i = 0; i < 11; i++)
            {
                int value = (int)Math.Pow(2, i + 1);

                var uri = new Uri("ms-appx:/Assets/" + value.ToString() + ".png", UriKind.RelativeOrAbsolute);
                tiles[i] = new BitmapImage();
                tiles[i].UriSource = uri;
            }
        }

        private void gameGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!animating)
                animating = true;
            else
                return;

            PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);

            old_X = pp.RawPosition.X;
            old_Y = pp.RawPosition.Y;

            if (!animating)
                animating = true;

        }

        private void gameGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!animating)
                return;

            ushort[,] temp_board = game.getBoard();
            bool horizontal = false;
            PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);

            double new_X, new_Y;
            new_X = pp.RawPosition.X;
            new_Y = pp.RawPosition.Y;

            if (new_X - old_X == 0 && new_Y - old_Y == 0)
                return;


            if (Math.Abs(old_X - new_X) > Math.Abs(old_Y - new_Y))
            {
                horizontal = true;
            }

            if (horizontal)
            {
                if (new_X - old_X > 0)
                {
                    //Positivo horizontal
                    game.moveRight();
                }
                else
                {
                    //negativo horizontal 
                    game.moveLeft();
                }
            }
            else
            {
                if (new_Y - old_Y > 0)
                {
                    //Positivo vertical
                    game.moveDown();
                }
                else
                {
                    //negativo vertical 
                    game.moveUp();
                }
            }

            UpdateGrid();
            animating = false;
        }

        private void Restart_Button_Click(object sender, RoutedEventArgs e)
        {
            hideOverlay();
            ResetGame();
        }

        private void Exit_Buttom_Click(object sender, RoutedEventArgs e)
        {
            hideOverlay();
            Frame.Navigate(typeof(MainPage));
        }

        private void ShareFB_Buttom_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void ShareTW_Buttom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Frame.Navigate(typeof(MainPage));
        }

        private void ResetGame(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void ResetGame()
        {
            timer.Stop();
            game.resetBoard();
            game.addStartTiles();

            tMins = 0;
            tSecs = 0;

            timer.Start();

            UpdateGrid();
        }

        private void initializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            // Crappy documentation strikes again
            timer.Tick += new EventHandler<object>(timerFired);

            tMins = 0;
            tSecs = 0;
        }

        private void timerFired(object sender, object e)
        {
            if (tSecs == 59)
            {
                tSecs = 0;
                tMins++;
            }
            else
            {
                tSecs++;
            }

            //CHECK IF OPPONENT HAS LOST OR WON
            if (tSecs % 2 == 0 && multiplayer)
            {
                LobbyPage.getPlayer(LobbyPage.received_invit_name);

                if (opponent == null)
                {
                    /*timer.Stop();
                    LobbyPage.deletePlayer(LobbyPage.main_user[0].Value);
                    LobbyPage.received_invit_name = "";
                    showSubmitMenu("You Win!");*/
                }
                else
                {
                    for (int i = 0; i < opponent.Count; i++)
                    {
                        if (opponent[i].Key == "hasLost")
                        {
                            if (opponent[i].Value == "yes")
                            {
                                timer.Stop();
                                LobbyPage.deletePlayer(LobbyPage.main_user[0].Value);
                                LobbyPage.deletePlayer(LobbyPage.received_invit_name);
                                LobbyPage.received_invit_name = "";
                                showSubmitMenu("You Win!");
                                break;
                            }
                        }

                        if (opponent[i].Key == "hasWon")
                        {
                            if (opponent[i].Value == "yes")
                            {
                                timer.Stop();                            
                                LobbyPage.deletePlayer(LobbyPage.main_user[0].Value);
                                LobbyPage.deletePlayer(LobbyPage.received_invit_name);
                                LobbyPage.received_invit_name = "";
                                showSubmitMenu("You Lost...");
                                break;
                            }
                        }
                    }
                }
            }

            if (tSecs > 9)
            {
                timer_value.Text = tMins.ToString() + ":" + tSecs.ToString();
            }
            else
            {
                timer_value.Text = tMins.ToString() + ":0" + tSecs.ToString();
            }

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void showOverlay()
        {

            this.restart_buttom.Visibility = Visibility.Visible;
            this.lose_buttom.Visibility = Visibility.Visible;

            this.lose_panel.Visibility = Visibility.Visible;

            this.lose_text_block.Visibility = Visibility.Visible;
            this.lose_share_text_block.Visibility = Visibility.Visible;

            this.sharefb_buttom.Visibility = Visibility.Visible;
            this.sharetw_buttom.Visibility = Visibility.Visible;
        }

        private void hideOverlay()
        {
            if (multiplayer)
            {
                player_against_text_block.Visibility = Visibility.Collapsed;
                player_against_text_block.Text = default_text;
            }

            this.restart_buttom.Visibility = Visibility.Collapsed;
            this.lose_buttom.Visibility = Visibility.Collapsed;

            this.lose_panel.Visibility = Visibility.Collapsed;

            this.lose_text_block.Visibility = Visibility.Collapsed;
            this.lose_share_text_block.Visibility = Visibility.Collapsed;

            this.sharefb_buttom.Visibility = Visibility.Collapsed;
            this.sharetw_buttom.Visibility = Visibility.Collapsed;
        }

        private void showSubmitMenu(string gameState)
        {

            lose_text_block.Text = gameState;

            limit_text_block.Visibility = Visibility.Visible;
            this.lose_panel.Visibility = Visibility.Visible;
            lose_text_block.Visibility = Visibility.Visible;
            userTextHolder.Visibility = Visibility.Visible;
            submit_text_block.Visibility = Visibility.Visible;
            submit_button.Visibility = Visibility.Visible;
            cancel_button.Visibility = Visibility.Visible;

        }

        private void hideSubmitMenu()
        {
            limit_text_block.Visibility = Visibility.Collapsed;
            this.lose_panel.Visibility = Visibility.Collapsed;
            lose_text_block.Visibility = Visibility.Collapsed;
            userTextHolder.Visibility = Visibility.Collapsed;
            submit_text_block.Visibility = Visibility.Collapsed;
            submit_button.Visibility = Visibility.Collapsed;
            cancel_button.Visibility = Visibility.Collapsed;
        }


        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!tapped && !multiplayer) return;

            if (multiplayer)
            {
                userTextHolder.Text = LobbyPage.received_invit_name;
            }

            string nickname = userTextHolder.Text;
            if (nickname.Length == 0) return;

            if (nickname.Length > 6)
                nickname = nickname.Substring(0, 6);

            string time_to_complete = ((tMins * 60) + tSecs).ToString();
            string temp_score = (game.getScore() / 2).ToString();

            postHighScore(nickname, game.getScore().ToString(), time_to_complete);

            hideSubmitMenu();
            userTextHolder.Text = "Nickname";
            showOverlay();
        }

        private async void postHighScore(String user, String v, String t)
        {
            var uri = new Uri(MainPage.URL + "scores");
            var httpClient = new HttpClient(new HttpClientHandler());

            var values = new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>("username",user),
                    new KeyValuePair<string, string>("val",v),
                    new KeyValuePair<string, string>("ti",t) 
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

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            hideSubmitMenu();
            userTextHolder.Text = "Nickname";
            showOverlay();
        }

        private void userTextHolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            tapped = true;
        }
    }
}
