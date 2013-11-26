using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poker
{
    class Deck
    {
        private Card[] drawnCards;
        private Stack<Card> notDrawnCards;
        private int drawCounter;

        public Card draw() {
            Card drawn = notDrawnCards.Pop();
            drawnCards[drawCounter] = drawn;
            drawCounter++;
            return drawn;
        }
    }
}
