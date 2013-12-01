using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace poker
{
    /// <summary>
    /// Interaction logic for Play.xaml
    /// </summary>
    public partial class Play : Page
    {
        Game game;

        public Play()
        {
            game = new Game();
            game.newGame();
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
            DataContext = game;
        }

        private void selectCard(object sender, RoutedEventArgs e)
        {
            Image b = (Image)sender;
            char last = b.Tag.ToString().Last();
            int cardNumber = (int)last - '0';

            if (game.subsFinished())
            {
                Image playedCardImg = e.Source as Image;
                if (playedCardImg != null)
                {
                    game.playCard(cardNumber);
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p2card" + game.getP2Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p3card" + game.getP3Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p4card" + game.getP4Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                game.markCardForsub(cardNumber);
            //b.Opacity = 0.5; //todo
            }
        }

        private void pressSubBtn(object sender, RoutedEventArgs e)
        {
            game.doSub();

            // Hide sub button if sub rounds are done
            if(game.subsFinished())
            {
                Button source = e.Source as Button;
                if (source != null)
                    source.Visibility = Visibility.Hidden;
            }
        }
    }
}
