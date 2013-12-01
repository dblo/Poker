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
        readonly string[] suitsArray = { "h", "s", "d", "c" };
        Deck deck;
        Card[] player1, player2, player3, player4;
        Queue<int> cardsToSub;
        int subRound,
            p1_score,
            p2_score;
        BitmapImage[] cardImages;

        public Game()
        {
            deck = new Deck();
            player1 = new Card[CARDS_PER_HAND];
            player2 = new Card[CARDS_PER_HAND];
            player3 = new Card[CARDS_PER_HAND];
            player4 = new Card[CARDS_PER_HAND];
            cardsToSub = new Queue<int>();
            cardImages = new BitmapImage[53];
            loadImages();
        }

        public void newGame()
        {
            deck.shuffle();
            for (int i = 0; i < CARDS_PER_HAND; i++)
            {
                player1[i] = deck.draw();
                player2[i] = null;// deck.draw();
                player3[i] = null;// deck.draw();
                player4[i] = null;// deck.draw();
            }
            subRound = 1;
            p1_score = p2_score = 0;
        }

        public void loadImages()
        {
            string mediaPath = "C:/Users/First/Documents/Visual Studio 2013/Projects/poker/poker/Media/"; //todo
            cardImages[0] = new BitmapImage(new Uri(mediaPath + "Back.png"));

            for (int i = 1; i < 53; i++)
            {
                Card card = deck.getCardByIndex(i-1);
                cardImages[i] = new BitmapImage(new Uri(mediaPath + card.toString() + ".png"));
            }
        }

        private BitmapImage getImage(Card card)
        {
            if (card == null)
            // Return card backside
                return cardImages[0];

            int suitOffset = 0;
            switch (card.getSuit())
            {
                //case h => 0 offset

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

            suitOffset += card.getNumber();
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

            if(subsFinished())
                for (int i = 0; i < CARDS_PER_HAND; i++)
                {
                    player2[i] = deck.draw();
                    player3[i] = deck.draw();
                    player4[i] = deck.draw();

                    string s = "P2_Card" + (i+ 1).ToString();
                    OnPropertyChanged(s);
                    
                    s = "P3_Card" + (i + 1).ToString();
                    OnPropertyChanged(s);
                    
                    s = "P4_Card" + (i + 1).ToString();
                    OnPropertyChanged(s);
                }
        }   

        // Return true if no more substitutions are allowed
        public bool subsFinished()
        {
            return subRound > ALLOWED_SUBST;
        }

        //public void playCard(int cardNum)
        //{
        //    p1_played_index = cardNum - 1;
        //    OnPropertyChanged("P1_Played");
        //}

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

        //public String P1_Played
        //{
        //    get
        //    {
        //        if (p1_played_index >= 0)
        //            return player1[p1_played_index].toString();
        //        else
        //            return "";
        //    }
        //}

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
        }
        public BitmapImage P1_Card2
        {
            get { return getImage(player1[1]); }
        }
        public BitmapImage P1_Card3
        {
            get { return getImage(player1[2]); }
        }
        public BitmapImage P1_Card4
        {
            get { return getImage(player1[3]); }
        }
        public BitmapImage P1_Card5
        {
            get { return getImage(player1[4]); }
        }
        public BitmapImage P2_Card1
        {
            get { return getImage(player2[0]); }
        }
        public BitmapImage P2_Card2
        {
            get { return getImage(player2[1]); }
        }
        public BitmapImage P2_Card3
        {
            get { return getImage(player2[2]); }
        }
        public BitmapImage P2_Card4
        {
            get { return getImage(player2[3]); }
        }
        public BitmapImage P2_Card5
        {
            get { return getImage(player2[4]); }
        }
        public BitmapImage P3_Card1
        {
            get { return getImage(player3[0]); }
        }
        public BitmapImage P3_Card2
        {
            get { return getImage(player3[1]); }
        }
        public BitmapImage P3_Card3
        {
            get { return getImage(player3[2]); }
        }
        public BitmapImage P3_Card4
        {
            get { return getImage(player3[3]); }
        }
        public BitmapImage P3_Card5
        {
            get { return getImage(player3[4]); }
        }
        public BitmapImage P4_Card1
        {
            get { return getImage(player4[0]); }
        }
        public BitmapImage P4_Card2
        {
            get { return getImage(player4[1]); }
        }
        public BitmapImage P4_Card3
        {
            get { return getImage(player4[2]); }
        }
        public BitmapImage P4_Card4
        {
            get { return getImage(player4[3]); }
        }
        public BitmapImage P4_Card5
        {
            get { return getImage(player4[4]); }
        }
    }
}
