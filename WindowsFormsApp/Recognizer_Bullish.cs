using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    internal class Recognizer_Bullish : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Bullish() : base("Bullish", 1)
        {
        }

        // Abstract Method to indicate if recognition was successful
        public override bool Recognize(List<SmartCandlestick> scsList, int index)
        {
            // Checking if the pattern value already exists; if not calculate
            SmartCandlestick scs = scsList[index];
            if (scs.Dictionary_Pattern.TryGetValue(Pattern_Name, out bool value))
            {
                return value;
            }
            else
            {
                bool bullish = scs.close > scs.open;        // condition for bullish
                scs.Dictionary_Pattern.Add(Pattern_Name, bullish);      // adding to pattern dictionary
                return bullish;
            }
        }
    }
}
