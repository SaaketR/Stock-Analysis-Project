using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    internal class Recognizer_Marubozu : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Marubozu() : base("Marubozu", 1)
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
                bool marubozu = scs.bodyRange > (scs.range * 0.96m);        // condition for marubozu candlestick
                scs.Dictionary_Pattern.Add(Pattern_Name, marubozu);         // adding to pattern dictionary
                return marubozu;
            }
        }
    }
}
