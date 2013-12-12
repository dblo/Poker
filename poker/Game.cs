using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Timers;

namespace poker
{
    public class Game : INotifyPropertyChanged
    {
        private readonly int CARDS_PER_HAND = 5,
                  ALLOWED_SUBST = 2,
                  STICK_ROUNDS = 5,
                  NUM_OF_PLAYERS = 4,
                  PLAYER1 = 0,
                  PLAYER2 = 1,
                  DEFAULT_TIMER = 1000;

        private Deck deck;
        private Card[] p1Hand, p2Hand, player3, player4;
        private Card[] p1RemainingCards, p2RemainingCards;
        private Queue<int> cardsToSub;
        private BitmapImage[] cardImages;
        private int subRound, stickRound,
            onPlayer, // Who's turn it is
            lastPlayer, // Last player to play a card in a stick round
            firstPlayer; // Player who played the first card in a stick
        private int[] playerScore, playerPlayed;
        private System.Timers.Timer stickRoundPauseTimer;
        private Card followCard;
        
        public Game()
        {
            deck = new Deck();
            p1Hand = new Card[CARDS_PER_HAND];
            p2Hand = new Card[CARDS_PER_HAND];
            player3 = new Card[CARDS_PER_HAND];
            player4 = new Card[CARDS_PER_HAND];
            p1RemainingCards = new Card[CARDS_PER_HAND];
            p2RemainingCards = new Card[CARDS_PER_HAND];
            cardsToSub = new Queue<int>();
            playerScore = new int[NUM_OF_PLAYERS];
            playerPlayed = new int[CARDS_PER_HAND];
                
            // To store card backside at index 0
            cardImages = new BitmapImage[53];
            loadImages();
            setStickTimer(DEFAULT_TIMER);
        }

        public void newGame()
        {
            for (int i = 0; i < NUM_OF_PLAYERS; i++)
                playerScore[i] = 0;
            newRound();
        }

        public void newRound()
        {
            deck.shuffle();
            subRound = stickRound = 1;
            onPlayer = 1;
            lastPlayer = 4;
            firstPlayer = 1;

            // Set no card played yet
            for (int j = 0; j < NUM_OF_PLAYERS; j++)
                playerPlayed[j] = -1;

            for (int i = 0; i < CARDS_PER_HAND; i++)
            {
                p1Hand[i] = deck.draw();
                p2Hand[i] = deck.draw();
                player3[i] = deck.draw();
                player4[i] = deck.draw();
                p1RemainingCards[i] = p1Hand[i];
                p2RemainingCards[i] = p2Hand[i];

                for (int j = 1; j <= NUM_OF_PLAYERS; j++)
                {
                    // Notify gui to show new cards
                    string card = "P" + j.ToString() + "_Card" + (i + 1).ToString();
                    OnPropertyChanged(card);
                }
            }

            for (int j = 1; j <= NUM_OF_PLAYERS; j++)
            {
                // Notify gui to remove old played cards
                string s = "P" + j.ToString() + "_Played";
                OnPropertyChanged(s);
            }
        }

        public void setStickTimer(int time)
        {
            stickRoundPauseTimer = new System.Timers.Timer(time);
            stickRoundPauseTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public void loadImages()
        {
            cardImages[0] = new BitmapImage(new Uri("Media/Back.png", UriKind.Relative));
            for (int i = 1; i < 53; i++)
            {
                Card card = deck.getCardByIndex(i - 1);
                cardImages[i] = new BitmapImage(new Uri("Media/" + card.toString() + ".png", UriKind.Relative));
            }
        }

        // Called for computer players. If the round is not over, the cards in their hands will
        // be dispayed with the backsides up
        private BitmapImage getImageCompPlayer(Card card)
        {
            if (!roundOver())
            {
                // Return card backside
                 return cardImages[0];
            }
            return getImage(card);
        }

        // Return the image resource for param card
        private BitmapImage getImage(Card card)
        {
            int suitOffset = 0;
            switch (card.getSuit())
            {
                //For case 'h' the offset = 0

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

        public void markCardForSub(int cardNum)
        {
            cardsToSub.Enqueue(cardNum);
        }

        // Return an array containing all cards the player want to substitute
        public int[] getCardsToSub()
        {
            return cardsToSub.ToArray();
        }

        // Replace all substituted cards by drawing new ones and ask gui to update
        public void doSub()
        {
            while (cardsToSub.Count() > 0)
            {
                int cardIndex = cardsToSub.Dequeue() - 1;
                p1Hand[cardIndex] = deck.draw();

                string s = "P1_Card" + (cardIndex + 1).ToString();
                OnPropertyChanged(s);
            }
            subRound++;
        }

        public int getPlayedCard(int player)
        {
            return playerPlayed[player - 1] + 1;
        }

        private void showAllCompCards()
        {
            for (int i = 2; i < 5; i++)
                for (int j = 1; j <= 5; j++)
                {
                    string s = "P" + i.ToString() + "_Card" + j.ToString();
                    OnPropertyChanged(s);
                }
        }

        public void playCard(int cardNum)
        {
            playerPlayed[PLAYER1] = cardNum-1;
            p1RemainingCards[cardNum - 1] = null;
            OnPropertyChanged("P1_Played");
            onPlayer++;

            // New stickround has begun
            if (firstPlayer == 1)
            {
                followCard = p1Hand[cardNum - 1];
                compPlaySticks();
            }
            stickRound++;

            if (roundOver())
                showAllCompCards();
            else
                stickRoundPauseTimer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            stickRoundPauseTimer.Stop();
            onPlayer = 2;
            firstPlayer = 2;
            lastPlayer = 1;

            if (firstPlayer > 1)
            {
                compPlaySticks();
                followCard = p2Hand[playerPlayed[PLAYER2]];
            }
        }

        // Computer players will play 1 card each if it's their turn
        private void compPlaySticks()
        {
            playerPlayed[1]++;
            OnPropertyChanged("P" + 2.ToString() + "_Played");

            if(firstPlayer > 1)
                onPlayer = 1;
        }
        
        // Return true if no more substitutions are allowed
        public bool subsFinished()
        {
            return subRound > ALLOWED_SUBST;
        }
        public bool roundOver()
        {
            return stickRound > STICK_ROUNDS;
        }
        // Return true if it is the human players turn
        private bool playersTurn()
        {
            return onPlayer == 1;
        }
        private bool cardFollowsSuit(int cardNum)
        {
            String followThisSuit = followCard.getSuit();

            if (p1Hand[cardNum - 1].getSuit() == followThisSuit)
                return true; // Want to play card of proper suit

            foreach (Card card in p1RemainingCards)
                if (card != null && card.getSuit() == followThisSuit)
                    return false;

            // Have no card of proper suit, allow play
            return true; 
        }
        // Return true if player is allowed to play card
        public bool mayPlayCard(int cardNum)
        {
            if (playersTurn())
                if (firstPlayer == 1 || cardFollowsSuit(cardNum))
                    return true;
            return false;
        }

        public BitmapImage P1_Played
        {
            get
            {
                if (playerPlayed[0] >= 0)
                    return getImage(p1Hand[playerPlayed[0]]);
                return null;
            }
        }
        public BitmapImage P2_Played
        {
            get
            {
                if (playerPlayed[1] >= 0)
                    return getImage(p2Hand[playerPlayed[1]]);
                return null;
            }
        }
        public BitmapImage P3_Played
        {
            get
            {
                if (playerPlayed[2] >= 0)
                    return getImage(player3[playerPlayed[2]]);
                return null;
            }
        }
        public BitmapImage P4_Played
        {
            get
            {
                if (playerPlayed[3] >= 0)
                    return getImage(player4[playerPlayed[3]]);
                return null;
            }
        }
        public int P1_Score
        {
            get { return playerScore[0]; }
            set { playerScore[0] = value; OnPropertyChanged("P1_Score"); }
        }
        public int P2_Score
        {
            get { return playerScore[1]; }
            set { playerScore[1] = value; OnPropertyChanged("P2_Score"); }
        }
        public int P3_Score
        {
            get { return playerScore[2]; }
            set { playerScore[2] = value; OnPropertyChanged("P3_Score"); }
        }
        public int P4_Score
        {
            get { return playerScore[3]; }
            set { playerScore[3] = value; OnPropertyChanged("P4_Score"); }
        }
        public BitmapImage P1_Card1
        {
            get { return getImage(p1Hand[0]); }
        }
        public BitmapImage P1_Card2
        {
            get { return getImage(p1Hand[1]); }
        }
        public BitmapImage P1_Card3
        {
            get { return getImage(p1Hand[2]); }
        }
        public BitmapImage P1_Card4
        {
            get { return getImage(p1Hand[3]); }
        }
        public BitmapImage P1_Card5
        {
            get { return getImage(p1Hand[4]); }
        }
        public BitmapImage P2_Card1
        {
            get { return getImageCompPlayer(p2Hand[0]); }
        }
        public BitmapImage P2_Card2
        {
            get { return getImageCompPlayer(p2Hand[1]); }
        }
        public BitmapImage P2_Card3
        {
            get { return getImageCompPlayer(p2Hand[2]); }
        }
        public BitmapImage P2_Card4
        {
            get { return getImageCompPlayer(p2Hand[3]); }
        }
        public BitmapImage P2_Card5
        {
            get { return getImageCompPlayer(p2Hand[4]); }
        }
        public BitmapImage P3_Card1
        {
            get { return getImageCompPlayer(player3[0]); }
        }
        public BitmapImage P3_Card2
        {
            get { return getImageCompPlayer(player3[1]); }
        }
        public BitmapImage P3_Card3
        {
            get { return getImageCompPlayer(player3[2]); }
        }
        public BitmapImage P3_Card4
        {
            get { return getImageCompPlayer(player3[3]); }
        }
        public BitmapImage P3_Card5
        {
            get { return getImageCompPlayer(player3[4]); }
        }
        public BitmapImage P4_Card1
        {
            get { return getImageCompPlayer(player4[0]); }
        }
        public BitmapImage P4_Card2
        {
            get { return getImageCompPlayer(player4[1]); }
        }
        public BitmapImage P4_Card3
        {
            get { return getImageCompPlayer(player4[2]); }
        }
        public BitmapImage P4_Card4
        {
            get { return getImageCompPlayer(player4[3]); }
        }
        public BitmapImage P4_Card5
        {
            get { return getImageCompPlayer(player4[4]); }
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
    }
}
//[ValueConversion(typeof(Card), typeof(BitmapImage)]
//public class CardBitMapImageConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfoConverter culture)
//    {

//    }
//}