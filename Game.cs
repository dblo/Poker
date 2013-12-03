﻿using System;
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
                  ALLOWED_SUBST = 2,
                  STICK_ROUNDS = 5;
        readonly string[] suitsArray = { "h", "s", "d", "c" };
        Deck deck;
        Card[] player1, player2, player3, player4;
        Queue<int> cardsToSub;
        int subRound,
            p1_score,
            p2_score,
            p3_score,
            p4_score,
            p1Played,
            p2Played,
            p3Played,
            p4Played,
            stickRound;
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
            p1_score = p2_score = p3_score = p4_score = 0;
            newRound();
        }

        public void newRound()
        {
            deck.shuffle();
            subRound = 1;
            p1Played = p2Played = p3Played = p4Played = -1;
            stickRound = 1;

            for (int i = 0; i < CARDS_PER_HAND; i++)
            {
                player1[i] = deck.draw();
                player2[i] = deck.draw();
                player3[i] = deck.draw();
                player4[i] = deck.draw();

                for (int j = 1; j < 5; j++)
                {
                    string s = "P" + j.ToString() + "_Card" + (i + 1).ToString();
                    OnPropertyChanged(s);
                }
            }

            for (int j = 1; j < 5; j++)
            {
                string s = "P" + j.ToString() + "_Played";
                OnPropertyChanged(s);
            }
        }

        public void loadImages()
        {
            string mediaPath = "C:/Users/First/Documents/Visual Studio 2013/Projects/poker/poker/Media/"; //todo
            cardImages[0] = new BitmapImage(new Uri(mediaPath + "Back.png"));

            for (int i = 1; i < 53; i++)
            {
                Card card = deck.getCardByIndex(i - 1);
                cardImages[i] = new BitmapImage(new Uri(mediaPath + card.toString() + ".png"));
            }
        }

        private BitmapImage getImageComp(Card card)
        {
            if (!roundOver())
            {
                // Return card backside
                return cardImages[0];
            }
            return getImage(card);
        }

        private BitmapImage getImage(Card card)
        {
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

        public int[] doSub()
        {
            int[] subbedCards = cardsToSub.ToArray();
            while (cardsToSub.Count() > 0)
            {
                int cardIndex = cardsToSub.Dequeue() - 1;
                player1[cardIndex] = deck.draw();

                //Todo, not manual call of onprop..
                string s = "P1_Card" + (cardIndex + 1).ToString();
                OnPropertyChanged(s);
            }
            subRound++;
            return subbedCards;
        }

        // Return true if no more substitutions are allowed
        public bool subsFinished()
        {
            return subRound > ALLOWED_SUBST;
        }

        public void playCard(int cardNum)
        {
            stickRound++;
            p1Played = cardNum - 1;
            OnPropertyChanged("P1_Played");

            p2Played++;
            OnPropertyChanged("P2_Played");

            p3Played++;
            OnPropertyChanged("P3_Played");

            p4Played++;
            OnPropertyChanged("P4_Played");

            // Comp players showdown of cards
            if (roundOver())
            {
                for (int i = 2; i < 5; i++)
                    for (int j = 1; j <= 5; j++)
                    {
                        string s = "P" + i.ToString() + "_Card" + j.ToString();
                        OnPropertyChanged(s);
                    }
            }
        }

        public int getP2Played()
        {
            return p2Played + 1;
        }

        public int getP3Played()
        {
            return p3Played + 1;
        }

        public int getP4Played()
        {
            return p4Played + 1;
        }

        public bool roundOver()
        {
            return stickRound > STICK_ROUNDS;
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

        public BitmapImage P1_Played
        {
            get
            {
                if (p1Played >= 0)
                    return getImage(player1[p1Played]);
                return null;
            }
        }

        public BitmapImage P2_Played
        {
            get
            {
                if (p2Played >= 0)
                    return getImage(player2[p2Played]);
                return null;
            }
        }
        public BitmapImage P3_Played
        {
            get
            {
                if (p3Played >= 0)
                    return getImage(player3[p3Played]);
                return null;
            }
        }

        public BitmapImage P4_Played
        {
            get
            {
                if (p4Played >= 0)
                    return getImage(player4[p4Played]);
                return null;
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

        public int P3_Score
        {
            get { return p3_score; }
            set { p3_score = value; OnPropertyChanged("P3_Score"); }
        }

        public int P4_Score
        {
            get { return p4_score; }
            set { p4_score = value; OnPropertyChanged("P4_Score"); }
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
            get { return getImageComp(player2[0]); }
        }
        public BitmapImage P2_Card2
        {
            get { return getImageComp(player2[1]); }
        }
        public BitmapImage P2_Card3
        {
            get { return getImageComp(player2[2]); }
        }
        public BitmapImage P2_Card4
        {
            get { return getImageComp(player2[3]); }
        }
        public BitmapImage P2_Card5
        {
            get { return getImageComp(player2[4]); }
        }
        public BitmapImage P3_Card1
        {
            get { return getImageComp(player3[0]); }
        }
        public BitmapImage P3_Card2
        {
            get { return getImageComp(player3[1]); }
        }
        public BitmapImage P3_Card3
        {
            get { return getImageComp(player3[2]); }
        }
        public BitmapImage P3_Card4
        {
            get { return getImageComp(player3[3]); }
        }
        public BitmapImage P3_Card5
        {
            get { return getImageComp(player3[4]); }
        }
        public BitmapImage P4_Card1
        {
            get { return getImageComp(player4[0]); }
        }
        public BitmapImage P4_Card2
        {
            get { return getImageComp(player4[1]); }
        }
        public BitmapImage P4_Card3
        {
            get { return getImageComp(player4[2]); }
        }
        public BitmapImage P4_Card4
        {
            get { return getImageComp(player4[3]); }
        }
        public BitmapImage P4_Card5
        {
            get { return getImageComp(player4[4]); }
        }
    }
}
