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
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page {
        Game game;

        private Uri _imageSource;
        public Uri ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value;}
        }

        public MainPage()
        {
            this.InitializeComponent();
            //ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            game = new Game();

            UpdateGrid();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Image_Left_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.score_value.Text = "4";
            game.moveLeft();
            UpdateGrid();
        }

        private void Image_Right_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.score_value.Text = "2";
            game.moveRight();
            UpdateGrid();
        }

        private void Image_Top_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.score_value.Text = "1";
            game.moveUp();
            UpdateGrid();
        }

        private void Image_Bottom_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.score_value.Text = "3";
            game.moveDown();
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            gameGrid.Children.Clear();

            Tile[,] temp_board = game.getBoard();

            for (ushort i = 0; i < Game.boardSize; i++)
            {
                for (ushort j = 0; j < Game.boardSize; j++)
                {
                    if (!temp_board[i, j].getAvailability())
                    {
                        int tile_value = temp_board[i, j].getValue();
                        if(tile_value != 0)
                            addPiece(Game.representation[temp_board[i, j].getValue() - 1], j, i );
                    }

                }
            }
        }

        private void addPiece(String value, int x_pos, int y_pos)
        {
            Image temp_img = new Image();

            var uri = new Uri("ms-appx:/Assets/" + value + ".png", UriKind.RelativeOrAbsolute);
            BitmapImage myImage = new BitmapImage();
            myImage.UriSource = uri;

            temp_img.Source = myImage;

            Grid.SetColumn(temp_img, x_pos);
            Grid.SetRow(temp_img, y_pos);

            gameGrid.Children.Add(temp_img);
                       
        }

        //public void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    var velocities = e.Velocities;
        //}
    }
}
