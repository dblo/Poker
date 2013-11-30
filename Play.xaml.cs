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
    /// Image cards source http://www.google.se/imgres?imgurl=http://www.jfitz.com/cards/windows-playing-cards.png&imgrefurl=http://www.jfitz.com/cards/&usg=__2iWarkU2fscs1WHtG0XPh1YtDK0=&h=394&w=951&sz=20&hl=en&start=1&sig2=Xd1BfVoIcygWrCa0-6RkaQ&zoom=1&tbnid=WL2BNarcVaEn9M:&tbnh=61&tbnw=148&ei=j06KUu_CM-Wk4gTxvoHAAQ&itbs=1&sa=X&ved=
    public partial class Play : Page
    {
        Game game;

        public Play()
        {
            game = new Game();
            game.newGame();
            InitializeComponent();
            DataContext = game;
        }

        private void selectCard(object sender, RoutedEventArgs e)
        {
            Image b = (Image)sender;
            char last = b.Tag.ToString().Last();
            int cardNumber = (int)last - '0';

            if (game.subsFinished())
            {
                Image source = e.Source as Image;
                if (source != null)
                {
                    game.playCard(cardNumber);
                    source.Visibility = Visibility.Hidden;
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
