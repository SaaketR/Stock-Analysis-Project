using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    internal class Recognizer_Gravestone_Doji : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Gravestone_Doji() : base("Gravestone Doji", 1)
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
                bool gravestone = scs.upperTail > (scs.range * 0.66m);      // condition for gravestone
                bool doji = scs.bodyRange < (scs.range * 0.03m);        // condition for doji
                bool gravestone_doji = gravestone & doji;       // condition for gravestone-doji
                scs.Dictionary_Pattern.Add(Pattern_Name, gravestone_doji);      // adding to pattern dictionary
                return gravestone_doji;
            }
        }
    }
}
