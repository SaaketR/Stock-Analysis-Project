using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    internal class Recognizer_Dragonfly_Doji : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Dragonfly_Doji() : base("Dragonfly Doji", 1)
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
                bool dragonfly = scs.lowerTail > (scs.range * 0.66m);       // condition for dragonfly
                bool doji = scs.bodyRange < (scs.range * 0.03m);        // condition for doji
                bool dragonfly_doji = dragonfly & doji;     // condition for dragonfly-doji
                scs.Dictionary_Pattern.Add(Pattern_Name, dragonfly_doji);       // adding to pattern dictionary
                return dragonfly_doji;
            }
        }
    }
}
