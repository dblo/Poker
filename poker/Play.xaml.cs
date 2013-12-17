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
        private Game game;

        public Play()
        {
            game = new Game();
            game.newGame();
            InitializeComponent();

            // Make the cards look better
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
            DataContext = game;
        }

        //Hide cards played by computer opponents
        private void hideCompPlayedCards()
        {
            Image playedCardImg;
            for (int i = 2; i <= 2; i++) //todo limited to 2p
            {
                string s = "p" + i.ToString() + "card" +
                    game.getPlayedCard(i).ToString();
                playedCardImg = FindName(s) as Image;
                playedCardImg.Visibility = Visibility.Hidden;
            }
        }

        // Make images containing cards in hands visible for showdown at tend of round
        private void makeCardsVisible()
        {
            Image playedCardImg;
            for (int i = 1; i < 3; i++)
                for (int j = 1; j <= 5; j++)
                {
                    playedCardImg = FindName("p" + i.ToString() +
                        "card" + j.ToString()) as Image;
                    playedCardImg.Visibility = Visibility.Visible;
                }
        }

        // Mark card to be substituted or play a card depending on game stage
        private void selectCard(object sender, RoutedEventArgs e)
        {
            if (!game.roundOver())
            {
                Image selectedCard = (Image)sender;

                //Card number is stored at end of card tag
                char lastChar = selectedCard.Tag.ToString().Last();
                int cardNumber = lastChar - '0';

                if (game.subsFinished())
                {
                    if (game.mayPlayCard(cardNumber))
                    {
                        game.playCard(cardNumber);
                        selectedCard.Visibility = Visibility.Hidden;
                        hideCompPlayedCards();

                        if (game.roundOver())
                        {
                            makeCardsVisible();
                            Button btn = FindName("controlBtn") as Button;
                            btn.Content = "Next";
                            btn.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    game.markCardForSub(cardNumber);
                    selectedCard.Opacity = 0.5;
                }
            }
        }

        // Initiate cards substitution or start next round depending on game stage
        private void pressBtn(object sender, RoutedEventArgs e)
        {
            int[] toSubCards = game.getCardsToSub();
            game.doSub();

            foreach (int subbedCard in toSubCards)
            {
                Image subbedImage = FindName("p1card" + subbedCard.ToString()) as Image;
                subbedImage.Opacity = 1;
            }

            if (game.subsFinished())
            {
                Button btn = e.Source as Button;

                if (game.roundOver())
                {
                    game.newRound();
                    btn.Content = "Sub";
                    btn.Visibility = Visibility.Visible;
                }
                else
                {
                    btn.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
