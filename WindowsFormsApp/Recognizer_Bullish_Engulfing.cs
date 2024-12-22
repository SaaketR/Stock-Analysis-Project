using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    internal class Recognizer_Bullish_Engulfing : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Bullish_Engulfing() : base("Bullish Engulfing", 2)
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
                // Case for out of bounds 
                int offset = Pattern_Length / 2;
                if (index < offset)
                {
                    scs.Dictionary_Pattern.Add(Pattern_Name, false);
                    return false;
                }
                else
                {
                    SmartCandlestick prev = scsList[index - offset];
                    bool bullish = (prev.open > prev.close) & (scs.close > scs.open);       // condition for bullish
                    bool engulfing = (scs.topPrice > prev.topPrice) & (scs.bottomPrice < prev.bottomPrice);     // condition for engulfing
                    bool bullish_engulfing = bullish & engulfing;       // conditio nfor bullish-engulfing
                    scs.Dictionary_Pattern.Add(Pattern_Name, bullish_engulfing);        // adding to pattern dictionary
                    return bullish_engulfing;
                }
            }
        }
    }
}
