using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poker
{
    class Deck
    {
        private Card[] cards;
        private int top;
        private readonly string[] suits = { "h", "s", "d", "c" };

        public Deck()
        {
            cards = new Card[52];
            top = 0;
            makeDeck();
        }

        private void makeDeck()
        {
            int i = 0;
            foreach (string suit in suits)
                for (int j = 1; j <= 13; i++, j++)
                {
                    cards[i] = new Card(suit, j);
                }
        }

        public Card getCardByIndex(int index)
        {
            return cards[index];
        }

        public Card getCardByString(string str)
        {
            int i=0;
            while (cards[i].ToString() != str)
                i++;
            return cards[i];
        }

        // Returns 1 card
        public Card draw()
        {
            Card topCard = cards[top];
            top++;
            return topCard; 
        }

        public void shuffle()
        {
            Card tmp;
            Random rng = new Random();
            for (int i=0; i < 1000; i++)
            {
                int swap1 = rng.Next(0, 51),
                    swap2 = rng.Next(0, 51);
                 tmp = cards[swap1];
                 cards[swap1] = cards[swap2];
                 cards[swap2] = tmp;
            }
            top = 0;
        }
    }
}
