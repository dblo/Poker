using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace poker
{
    class Game : INotifyPropertyChanged
    {
        const int CARDS_PER_HAND = 5;
        Deck deck;
        Card[] player1;

        public Game()
        {
            deck    = new Deck();
            player1 = new Card[CARDS_PER_HAND];
        }

        public void newGame()
        {
            deck.shuffle();
            for(int i=0; i < CARDS_PER_HAND; i++)
            {
                player1[i] = deck.draw();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        
        public String Card1
        {
            get { return player1[0].toString(); }
            set
            {
                OnPropertyChanged("Card1");
            }
        }
        public String Card2
        {
            get { return player1[1].toString(); }
            set
            {
                OnPropertyChanged("Card2");
            }
        }
        public String Card3
        {
            get { return player1[2].toString(); }
            set
            {
                OnPropertyChanged("Card3");
            }
        }
        public String Card4
        {
            get { return player1[3].toString(); }
            set
            {
                OnPropertyChanged("Card4");
            }
        }
        public String Card5
        {
            get { return player1[4].toString(); }
            set
            {
                OnPropertyChanged("Card5");
            }
        }
    }
}
