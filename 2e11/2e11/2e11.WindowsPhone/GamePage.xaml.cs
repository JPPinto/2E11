using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _2e11
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class GamePage : Page
    {

        Game game;

        private double old_X;
        private double old_Y;
        private bool animating;
        private BitmapImage[] tiles;

        public GamePage()
        {
            this.InitializeComponent();
            preloadImages();
            //ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            game = new Game();
            animating = false;

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

            this.score_value.Text = (game.getScore()/2).ToString();
        }

        private void addPiece(ushort value, int x_pos, int y_pos)
        {
            Image temp_img = new Image();
            temp_img.Source = tiles[value -1];

            Grid.SetColumn(temp_img, x_pos);
            Grid.SetRow(temp_img, y_pos);

            gameGrid.Children.Add(temp_img);

        }

        private void preloadImages() {
            tiles = new BitmapImage[11];

            for (int i = 0; i < 11; i++) {
                int value = (int) Math.Pow(2,i+1);
                
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
                score_value.Text = "100";
                if (new_X - old_X > 0)
                {
                    //Positivo horizontal
                    game.moveRight();
                    score_value.Text = "2";
                }
                else
                {
                    //negativo horizontal 
                    game.moveLeft();
                    score_value.Text = "4";
                }
            }
            else
            {
                score_value.Text = "200";
                if (new_Y - old_Y > 0)
                {
                    //Positivo vertical
                    game.moveDown();
                    score_value.Text = "3";
                }
                else
                {
                    //negativo vertical 
                    game.moveUp();
                    score_value.Text = "1";
                }
            }

            UpdateGrid();
            animating = false;
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            game.resetBoard();
            game.addStartTiles();

            UpdateGrid();
        }

        //public void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    var velocities = e.Velocities;
        //}
    }
}
