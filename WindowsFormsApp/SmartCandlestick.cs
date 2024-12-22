using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    /// <summary>
    /// Smart Candlestick class containing range, topPrice, bottomPrice, bodyRange, and upperTail, and lowerTail attributes
    /// </summary>
    internal class SmartCandlestick : Candlestick
    {
        // Initialising the Smart Candlestick attributes
        public decimal range { get; set; }
        public decimal topPrice { get; set; }
        public decimal bottomPrice { get; set; }
        public decimal bodyRange { get; set; }
        public decimal upperTail { get; set; }
        public decimal lowerTail { get; set; }
        // Smart Candlestick dictionary to keep track of each pattern 
        public Dictionary<string, bool> Dictionary_Pattern = new Dictionary<string, bool>();

        //Inherit Constructor
        public SmartCandlestick(string csvLine) : base(csvLine)
        {
            // Calling the ComputeExtraProperties() and ComputePatternProperties() methods
            ComputeExtraProperties();
            ComputePatternProperties();
        }

        // Conversion Constructor
        public SmartCandlestick(Candlestick cs)
        {
            // Locally storing the attributes of the Candlestick class
            date = cs.date;
            open = cs.open;
            close = cs.close;
            high = cs.high;
            low = cs.low;
            volume = cs.volume;
            // Calling the ComputeExtraProperties() and ComputePatternProperties() methods
            ComputeExtraProperties();
            ComputePatternProperties();
        }

        //OPTIONAL: MAKE SCS CONSTRUCTOR FOR TEST

        /// <summary>
        /// Calculates the range, top/bottom price, body range, and upper/lower tails of the candlestick
        /// </summary>
        private void ComputeExtraProperties()
        {
            range = high - low;
            topPrice = Math.Max(open, close);
            bottomPrice = Math.Min(open, close);
            bodyRange = topPrice - bottomPrice;
            upperTail = high - topPrice;
            lowerTail = bottomPrice - low;
        }

        /// <summary>
        /// Method to assess the properties of the smart candlestick and store the patterns in the member dictionary
        /// </summary>
        private void ComputePatternProperties()
        {
            // Calculating if the candlestick is bullish in nature
            bool bullish = close > open;
            Dictionary_Pattern.Add("Bullish", bullish);

            // Calculating if the candlestick is bearish in nature
            bool bearish = open > close;
            Dictionary_Pattern.Add("Bearish", bearish);

            // Calculating if the candlestick is neutral in nature
            bool neutral = bodyRange < (range * 0.03m);
            Dictionary_Pattern.Add("Neutral", neutral);

            // Calculating if the candlestick is a marubozu
            bool marubozu = bodyRange > (range * 0.96m);
            Dictionary_Pattern.Add("Marubozu", marubozu);

            // Hammer min and max range
            bool hammer = ((range * 0.20m) < bodyRange) & (bodyRange < (range * 0.33m)) & (lowerTail > range * 0.66m);
            Dictionary_Pattern.Add("Hammer", hammer);

            // Calculating if the candlestick is a doji
            bool doji = bodyRange < (range * 0.03m);
            Dictionary_Pattern.Add("Doji", doji);

            // Calculating if the candlestick is a Dragonfly Doji
            bool dragonfly_doji = doji & (lowerTail > range * 0.66m);
            Dictionary_Pattern.Add("Dragonfly Doji", dragonfly_doji);

            // Calculating if the candlestick is a Gravestone Doji
            bool gravestone_doji = doji & (upperTail > range * 0.66m);
            Dictionary_Pattern.Add("Gravestone Doji", gravestone_doji);
        }
    }
}
