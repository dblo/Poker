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
            if (!game.roundOver())
            {
                Image selectedCard = (Image)sender;
                char last = selectedCard.Tag.ToString().Last();
                int cardNumber = (int)last - '0';

                if (game.subsFinished())
                {
                    Image playedCardImg;
                    game.playCard(cardNumber);
                    playedCardImg = FindName("p1card" + cardNumber.ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p2card" + game.getP2Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p3card" + game.getP3Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    playedCardImg = FindName("p4card" + game.getP4Played().ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Hidden;

                    if (game.roundOver())
                    {
                        for (int i = 1; i < 5; i++)
                            for (int j = 1; j <= 5; j++)
                            {
                                playedCardImg = FindName("p" + i.ToString() + "card" + j.ToString()) as Image;
                                playedCardImg.Visibility = Visibility.Visible;
                            }

                        Button btn = FindName("controlBtn") as Button;
                        btn.Content = "Next";
                        btn.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    game.markCardForsub(cardNumber);
                    selectedCard.Opacity = 0.5;
                }
            }
        }

        private void pressSubBtn(object sender, RoutedEventArgs e)
        {
            foreach (int subbedCard in game.doSub())
            {
                Image subbedImage = FindName("p1card" + subbedCard.ToString()) as Image;
                subbedImage.Opacity = 1;
            }

            // Hide sub button if sub rounds are done
            if (game.subsFinished())
            {
                Button btn = e.Source as Button;
                if (btn != null)
                    btn.Visibility = Visibility.Hidden;

                if (game.roundOver())
                {
                    game.newRound();
                    btn.Content = "Sub";
                    btn.Visibility = Visibility.Visible;
                }
            }
        }
    }
}

