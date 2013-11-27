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
        public Play()
        {
            Game game = new Game();
            game.newGame();
            DataContext = game;
            InitializeComponent();
        }

        private void selectCard(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            char c = b.Tag.ToString().Last();
            MessageBox.Show(c.ToString());
        }
    }
}
