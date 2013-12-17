using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace poker
{
    public class Card
    {
        private string suit;
        private int number;

        public Card(string _suit, int _number)
        {
            suit = _suit;

            if(_number == 1)
                number = 13;
            number = _number;
        }

        public String toString()
        {
            return suit + number.ToString();
        }

        public bool sameSuit(Card comp)
        {
            return this.suit == comp.suit;
        }

        public bool isLower(Card comp)
        {
            return this.number < comp.number;
        }

        public string getSuit() { return suit; }
        public int getNumber() { return number; }
    }
}
