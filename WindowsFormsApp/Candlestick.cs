using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp_COP_4365
{
    internal class Candlestick
    {
        /// <summary>
        /// Candlestick class to create ticker objects based on a row of data derived from the the stock chosen by the user.
        /// Determines the opening and closing value of the stock, the high and low value during the given time period, as
        /// well as the volume of the stock and the date of the ticker.
        /// </summary>
        // Candlestick attribute storing the opening value of the stock ticker
        public decimal open { get; set; }
        // Candlestick attribute storing the highest value of the stock ticker
        public decimal high { get; set; }
        // Candlestick attribute storing the lowest value of the stock ticker
        public decimal low { get; set; }
        // Candlestick attribute storing the closing value of the stock ticker
        public decimal close { get; set; }
        // Candlestick attribute storing the volume of the stock ticker
        public ulong volume { get; set; }
        // Candlestick attribute storing the date of the stock ticker
        public DateTime date { get; set; }

        /// <summary>
        /// Constructor for the candlestick class. Accepts a row of data from the .csv file, hence parses the data to
        /// obtain the open/close, high/low, and volume values, as well as the date of the ticker. The row of data is 
        /// separated based on a few pre-specified delimiters and is, thus, parsed into the appropriate attributes.
        /// </summary>
        /// <param name="rowOfData">"String of data obtained from the Stock Data directory; data is from a .csv file"</param>
        public Candlestick()
        {
        }

        //String Line Constructor
        public Candlestick(string csvLine)
        {
            // Parsing the row of data based on the delimiters
            char[] seperators = new char[] { ',', ' ', '"' };       // list of delimiters based on which we will be separating
            string[] subs = csvLine.Split(seperators, StringSplitOptions.RemoveEmptyEntries);       // using the <string>.split() method to separate the row of data; also removing any empty entries for missing data

            // Extracting the date then sending to DateTime.Parse
            string dateString = subs[0];        // initialising a string to store the date
            date = DateTime.Parse(dateString);      // parsing the date string; converts the string representation of a date to its DateTime equivalent

            // Storing the data after separation into the attributes of the Candlestick class
            decimal temp;       // temporary variable to check if the separated variable is parasable
            bool success = decimal.TryParse(subs[1], out temp);     // checking if subs[1] is parsable
            if (success) open = temp;       // assigning subs[1] to Candlesticks object's open attribute

            success = decimal.TryParse(subs[2], out temp);      // checking if subs[2] is parsable
            if (success) high = temp;       // assigning subs[2] to Candlesticks object's high attribute

            success = decimal.TryParse(subs[3], out temp);      // checking if subs[3 is parsable
            if (success) low = temp;       // assigning subs[3] to Candlesticks object's low attribute

            success = decimal.TryParse(subs[4], out temp);      // checking if subs[4] is parsable
            if (success) close = temp;       // assigning subs[4] to Candlesticks object's close attribute

            ulong tempVolume;       // Creating a temporary variable for volume of type unsigned long (instead of decimal, like the other attributes)
            success = ulong.TryParse(subs[6], out tempVolume);  // checking if subs[6] is parsable
            if (success) volume = tempVolume;       // assigning subs[6] to Candlesticks object's volume attribute
        }
    }
}
