using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    internal class Recognizer_Bullish_Harami : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Bullish_Harami() : base("Bullish Harami", 2)
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
                    bool harami = (scs.topPrice < prev.topPrice) & (scs.bottomPrice > prev.bottomPrice);        // condition for harami
                    bool bullish_harami = bullish & harami;     // condition for bullish-harami
                    scs.Dictionary_Pattern.Add(Pattern_Name, bullish_harami);       // adding to pattern dictionary
                    return bullish_harami;
                }
            }
        }
    }
}
