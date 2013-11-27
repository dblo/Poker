using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace poker
{
    //enum suit

    class Card
    {
        private string  suit;
        private int     number;

        public Card(string _suit, int _number)
        {
            this.suit = _suit;
            this.number = _number;
        }

        public String toString()
        {
            return suit + number.ToString();
        }

        //public string Suit { get; }
        //public int Number { get; }
            public string getSuit() { return suit; }
            public int getNumber() { return number; }
    }
}
