using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    /// <summary>
    /// Recognizer class keeping track of the pattern names and lengths
    /// </summary>
    internal abstract class Recognizer
    {
        // Abstract Properties
        public string Pattern_Name;
        public int Pattern_Length;

        // Constructor method
        protected Recognizer(string pN, int pL)
        {
            Pattern_Name = pN;      // setting patern name to pN
            Pattern_Length = pL;    // setting pattern length to pL
        }

        // Abstract Method to indicate if recognition was successful
        public abstract bool Recognize(List<SmartCandlestick> scsList, int index);

        // Concrete method calling the recognize method on all smart candlesticks
        public void Recognize_All(List<SmartCandlestick> scsList)
        {
            // Iterating through all smart candlesticks
            for (int i = 0; i < scsList.Count; i++)
            {
                Recognize(scsList, i);      // calling the recognize method
            }
        }
    }
}