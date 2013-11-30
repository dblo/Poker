using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace poker
{
    class Game : INotifyPropertyChanged
    {
        readonly int CARDS_PER_HAND = 5,
                  ALLOWED_SUBST = 2;
        readonly string[] suitsArray = {"h", "s", "d", "c"};
        Deck deck;
        Card[] player1, player2;
        Queue<int> cardsToSub;
        int subRound,
            p1_played_index,
            p1_score,
            p2_score;
        BitmapImage[] cardImages;

        public Game()
        {
            deck = new Deck();
            player1 = new Card[CARDS_PER_HAND];
            player2 = new Card[CARDS_PER_HAND];
            cardsToSub = new Queue<int>();
            cardImages = new BitmapImage[52];
            loadImages();
        }

        public void newGame()
        {
            deck.shuffle();
            for (int i = 0; i < CARDS_PER_HAND; i++)
            {
                player1[i] = deck.draw();
                player2[i] = deck.draw();
            }
            subRound = 1;
            p1_played_index = -1;
            p1_score = p2_score = 0;
        }

        public void loadImages()
        {
            string mediaPath = "C:/Users/First/Documents/Visual Studio 2013/Projects/poker/poker/Media/"; //todo
            for (int i = 0; i < 52; i++ )
            {
                Card card = deck.getCardByIndex(i);
                cardImages[i] = new BitmapImage(new Uri(mediaPath + card.toString() + ".png"));
            }
        }

        private BitmapImage getImage(Card card)
        {
            int suitOffset = 0;
            switch(card.getSuit())
            {
                //case 'h':
                //    suitOffset = 0;
                //    break;

                case "s":
                suitOffset = 13;
                break;

                case "d":
                suitOffset = 26;
                break;

                case "c":
                    suitOffset = 39;
                    break;
            }

            suitOffset += card.getNumber() - 1;
            return cardImages[suitOffset];
        }
        
        public void markCardForsub(int cardNum)
        {
            cardsToSub.Enqueue(cardNum);
        }

        public void doSub()
        {
            while (cardsToSub.Count() > 0)
            {
                int cardIndex = cardsToSub.Dequeue() - 1;
                player1[cardIndex] = deck.draw();

                //Todo, not manual call of onprop..
                string s = "P1_Card" + (cardIndex + 1).ToString();
                OnPropertyChanged(s);
            }
            subRound++;
            //printHandToDebug();
        }

        // Return true if no more substitutions are allowed
        public bool subsFinished()
        {
            return subRound > ALLOWED_SUBST;
        }

        public void playCard(int cardNum)
        {
            p1_played_index = cardNum - 1;
            OnPropertyChanged("P1_Played");
        }

        // Debug util
        private void printHandToDebug()
        {
            foreach (Card c in player1)
                System.Diagnostics.Debug.Print(c.toString());
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

        public String P1_Played
        {
            get
            {
                if (p1_played_index >= 0)
                    return player1[p1_played_index].toString();
                else
                    return "";
            }
        }

        public int P1_Score
        {
            get { return p1_score; }
            set { p1_score = value; OnPropertyChanged("P1_Score"); }
        }

        public int P2_Score
        {
            get { return p2_score; }
            set { p2_score = value; OnPropertyChanged("P2_Score"); }
        }

        public BitmapImage P1_Card1
        {
            get { return getImage(player1[0]); }
            //set
            //{
            //    OnPropertyChanged("Card1");
            //}
        }
        public BitmapImage P1_Card2
        {
            get { return getImage(player1[1]); }
            //set
            //{
            //    OnPropertyChanged("Card2");
            //}
        }
        public BitmapImage P1_Card3
        {
            get { return getImage(player1[2]); }
            //set
            //{
            //    OnPropertyChanged("Card3");
            //}
        }
        public BitmapImage P1_Card4
        {
            get { return getImage(player1[3]); }
            //set
            //{
            //    OnPropertyChanged("Card4");
            //}
        }
        public BitmapImage P1_Card5
        {
            get { return getImage(player1[4]); }
            //set
            //{
            //    OnPropertyChanged("Card5");
            //}
        }
        public BitmapImage P2_Card1
        {
            get { return getImage(player2[0]); }
            //set
            //{
            //    OnPropertyChanged("Card1");
            //}
        }
        public BitmapImage P2_Card2
        {
            get { return getImage(player2[1]); }
            //set
            //{
            //    OnPropertyChanged("Card2");
            //}
        }
        public BitmapImage P2_Card3
        {
            get { return getImage(player2[2]); }
            //set
            //{
            //    OnPropertyChanged("Card3");
            //}
        }
        public BitmapImage P2_Card4
        {
            get { return getImage(player2[3]); }
            //set
            //{
            //    OnPropertyChanged("Card4");
            //}
        }
        public BitmapImage P2_Card5
        {
            get { return getImage(player2[4]); }
            //set
            //{
            //    OnPropertyChanged("Card5");
            //}
        }
    }
}
