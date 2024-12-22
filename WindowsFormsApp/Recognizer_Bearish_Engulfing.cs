using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    internal class Recognizer_Bearish_Engulfing : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Bearish_Engulfing() : base("Bearish Engulfing", 2)
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
                    bool bearish = (prev.open < prev.close) & (scs.close < scs.open);       // condition for bearish
                    bool engulfing = (scs.topPrice > prev.topPrice) & (scs.bottomPrice < prev.bottomPrice);     // condition for engulfing
                    bool bearish_engulfing = bearish & engulfing;       // condition for bearing-engulfing
                    scs.Dictionary_Pattern.Add(Pattern_Name, bearish_engulfing);    // adding to pattern dictionary
                    return bearish_engulfing;
                }
            }
        }
    }
}
