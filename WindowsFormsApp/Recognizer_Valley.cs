﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    internal class Recognizer_Valley : Recognizer
    {
        // Inheriting constructor from base class
        public Recognizer_Valley() : base("Valley", 3)
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
                // Checking if the pattern value already exists; if not calculate
                int offset = Pattern_Length / 2;
                if ((index < offset) | (index == scsList.Count() - offset))
                {
                    scs.Dictionary_Pattern.Add(Pattern_Name, false);
                    return false;
                }
                else
                {
                    SmartCandlestick prev = scsList[index - offset];        // previous value of smart candlestick
                    SmartCandlestick next = scsList[index + offset];        // next value of smart candlestick
                    bool valley = (scs.low < prev.low) & (scs.low < next.low);      // condition for valley candlestick
                    scs.Dictionary_Pattern.Add(Pattern_Name, valley);       // adding to pattern dictionary
                    return valley;
                }
            }
        }
    }
}
