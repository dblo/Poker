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

//Console.Writeline("hi", var);

namespace poker
{
    public class Game : INotifyPropertyChanged
    {
        private readonly int CARDS_PER_HAND = 5,
                  ALLOWED_SUBST = 2,
                  STICK_ROUNDS = 5,
                  NUM_OF_PLAYERS = 4;

        private Deck deck;
        private Card[] player1, player2, player3, player4;
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
            player1 = new Card[CARDS_PER_HAND];
            player2 = new Card[CARDS_PER_HAND];
            player3 = new Card[CARDS_PER_HAND];
            player4 = new Card[CARDS_PER_HAND];
            cardsToSub = new Queue<int>();
            playerScore = new int[NUM_OF_PLAYERS];
            playerPlayed = new int[CARDS_PER_HAND];
            stickRoundPauseTimer = new System.Timers.Timer(2000);
            //stickRoundPauseTimer.Enabled = true;
            stickRoundPauseTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                
            // To store card backside at index 0
            cardImages = new BitmapImage[53];
            loadImages();
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
                player1[i] = deck.draw();
                player2[i] = deck.draw();
                player3[i] = deck.draw();
                player4[i] = deck.draw();

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
                player1[cardIndex] = deck.draw();

                string s = "P1_Card" + (cardIndex + 1).ToString();
                OnPropertyChanged(s);
            }
            subRound++;
        }

        public void playCard(int cardNum)
        {
            stickRound++;
            playerPlayed[0] = cardNum-1;
            onPlayer++;
            OnPropertyChanged("P1_Played");
            followCard = player1[cardNum - 1];

            compPlaySticks();

            if (roundOver())
            {
                // Comp players showdown of cards
                for (int i = 2; i < 5; i++)
                    for (int j = 1; j <= 5; j++)
                    {
                        string s = "P" + i.ToString() + "_Card" + j.ToString();
                        OnPropertyChanged(s);
                    }
            }
            else
            {
                stickRoundPauseTimer.Start();
            }
        }

        // Computer players will play 1 card each if it's their turn
        private void compPlaySticks()
        {
            // Pretend player 1 is player 5 for easier looping
            int last = (lastPlayer > 1) ? lastPlayer : 4;

            for (int i = onPlayer - 1; i < last; i++)
            {
                playerPlayed[i]++;
                OnPropertyChanged("P" + (i + 1).ToString() + "_Played");
                onPlayer++;
            }
            if (onPlayer > NUM_OF_PLAYERS)
                onPlayer = 1;
        }
        
        public int getPlayedCard(int player)
        {
            return playerPlayed[player-1] + 1;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            onPlayer = 2;
            firstPlayer = 2;
            lastPlayer = 1; // (onPlayer + 3) % NUM_OF_PLAYERS;
            if (onPlayer > 1)
                compPlaySticks();
            stickRoundPauseTimer.Stop();
        }

        // Rules

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
            return player1[cardNum-1].getSuit() == followCard.getSuit();
        }

        public bool mayPlayCard(int cardNum)
        {
            if (playersTurn())
                if (firstPlayer == 1 || cardFollowsSuit(cardNum))
                    return true;
            return false;
        }

        // end of rules

        public BitmapImage P1_Played
        {
            get
            {
                if (playerPlayed[0] >= 0)
                    return getImage(player1[playerPlayed[0]]);
                return null;
            }
        }

        public BitmapImage P2_Played
        {
            get
            {
                if (playerPlayed[1] >= 0)
                    return getImage(player2[playerPlayed[1]]);
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
            get { return getImageCompPlayer(player2[0]); }
        }
        public BitmapImage P2_Card2
        {
            get { return getImageCompPlayer(player2[1]); }
        }
        public BitmapImage P2_Card3
        {
            get { return getImageCompPlayer(player2[2]); }
        }
        public BitmapImage P2_Card4
        {
            get { return getImageCompPlayer(player2[3]); }
        }
        public BitmapImage P2_Card5
        {
            get { return getImageCompPlayer(player2[4]); }
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
