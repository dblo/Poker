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

        public Deck()
        {
            cards = new Card[52];
            top = 0;
            
            string[] suits = {"h", "s", "d", "c"};
            int i = 0;
            foreach (string suit in suits)
                for (int j = 1; j < 14; j++)
                {
                    cards[i] = new Card(suit, j);
                    i++;
                }
        }

        public Card getCardByIndex(int index)
        {
            return cards[index];
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
            Random rng = new Random();
            Card tmp;
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
