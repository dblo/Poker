using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace poker
{
    class Game : INotifyPropertyChanged
    {
        const int CARDS_PER_HAND = 5,
                  ALLOWED_SUBST = 2;

        Deck deck;
        Card[] player1, player2;
        Queue<int> cardsToSub;
        int subRound,
            p1_played_index,
            p1_score,
            p2_score,
            whoseTurn;
        BitmapImage sprites;

        public Game()
        {
            deck    = new Deck();
            player1 = new Card[CARDS_PER_HAND];
            player2 = new Card[CARDS_PER_HAND];
            cardsToSub = new Queue<int>();
            loadImages();
        }

        public void newGame()
        {
            deck.shuffle();
            for(int i=0; i < CARDS_PER_HAND; i++)
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
//            Uri src = new Uri("cards.png", UriKind.Relative);
//            Uri src = new Uri("C:\Users\First\Documents\Visual Studio 2013\Projects\poker\poker\cards.png", UriKind.Absolute);

//            sprites = new BitmapImage(src);
        }

        public ImageSource Test
        {
            get { return sprites; }
        }

        public int getWhoseTurn()
        {
            return whoseTurn;
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
        
        public String P1_Card1
        {
            get { return player1[0].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card1");
            //}
        }
        public String P1_Card2
        {
            get { return player1[1].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card2");
            //}
        }
        public String P1_Card3
        {
            get { return player1[2].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card3");
            //}
        }
        public String P1_Card4
        {
            get { return player1[3].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card4");
            //}
        }
        public String P1_Card5
        {
            get { return player1[4].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card5");
            //}
        }
        public String P2_Card1
        {
            get { return player2[0].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card1");
            //}
        }
        public String P2_Card2
        {
            get { return player2[1].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card2");
            //}
        }
        public String P2_Card3
        {
            get { return player2[2].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card3");
            //}
        }
        public String P2_Card4
        {
            get { return player2[3].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card4");
            //}
        }
        public String P2_Card5
        {
            get { return player2[4].toString(); }
            //set
            //{
            //    OnPropertyChanged("Card5");
            //}
        }
    }
}
