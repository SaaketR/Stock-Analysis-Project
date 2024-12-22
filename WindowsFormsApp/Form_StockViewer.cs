using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp_COP_4365
{
    /// <summary>
    /// Form class containing methods to load a stock, filter candlestick data, update stock data, displaying candlestick
    /// data, as well as normalizing the chart. Contains other helper methods that can read data from a file, perform an event
    /// on a button click, etc.
    /// </summary>
    public partial class Form_StockViewer : Form
    {
        
        private List<SmartCandlestick> candlesticks = null;      //List of all candlesticks read from file
        private BindingList<SmartCandlestick> boundCandlesticks = null;      // Creating an empty binding list of Candlestick objects bound to the Data Grid View         
        private DateTime startDate = new DateTime(2022, 1, 1);      // Storing the start date for stock data to be displayed. By default, set to 1st January, 2022
        private DateTime endDate = DateTime.Now;        // Storing the end date for stock data to be displayed. By default, set to present day
        private Dictionary<string, Recognizer> Dictionary_Recognizer;       // Storing all recognizers in a dictionary
        private double chartMax;        // highest chart value
        private double chartMin;        // lowest chart value

        /// <summary>
        /// Default constructor of the Form class. Initializes a list of Candlestick objects, as well the date time picker's
        /// start and end dates
        /// </summary>
        public Form_StockViewer()
        {
            InitializeComponent();
            InitializeRecognizer();

            candlesticks = new List<SmartCandlestick>(1024);     // Creating a list of Candlesticks objects; size of list 1024 bytes
            dateTimePicker_startDate.Value = startDate;     // Initializing the date time picker's start value
            dateTimePicker_endDate.Value = endDate;     // Initializing the date time picker's end value
        }

        /// <summary>
        /// Constructor of the child form
        /// </summary>
        /// <param name="stockPath">"File path of the child form"</param>
        /// <param name="start">"Start date"</param>
        /// <param name="end">"End date"</param>
        public Form_StockViewer(string stockPath, DateTime start, DateTime end)
        {
            InitializeComponent();
            InitializeRecognizer();

            dateTimePicker_startDate.Value = startDate = start;     // Obtaining start date from the parent
            dateTimePicker_endDate.Value = endDate = end;       // Obtaining end date from the parent
            candlesticks = goReadFile(stockPath);       // Calling goReadFile() method to read the child data
            filterList();       // Fitering child data using filterList() method
            displayCandlesticks();      // Displaying child data using displayCandlesticks() method
        }

        /// <summary>
        /// Event when the user triggers the open file button in the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_openFile_Click(object sender, EventArgs e)
        {
            Text = "Opening File...";       // Changing the text of the window form on opening
            openFileDialog_stockPick.ShowDialog();      // Displaying the file explorer
        }

        /// <summary>
        /// Updating the displayed chart data to the user specified time periodd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Update_Click(object sender, EventArgs e)
        {
            // Checking if the user has not inputted any data to be filtered and if the date is valied (i.e., start date should be before the end date)
            if ((candlesticks.Count != 0) & (startDate <= endDate))
            {
                filterList();       // calling the filterList() method to filter the candlesticks list
                displayCandlesticks();      // displaying the candlestick information as per the requirements
            }
        }

        /// <summary>
        /// Opening the file dialog event to allow the user to select the stock file, hence parsing the data to be 
        /// stored in a list, then displaying the candlestick as per our requirement ( data grid view and chart areas)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog_stockPick_FileOk(object sender, CancelEventArgs e)
        {
            
            int numberOfFiles = openFileDialog_stockPick.FileNames.Count();     // Keeping track of the number of files openned
            
            // Iterating through all the files openned by the file dialog 
            for (int i = 0; i < numberOfFiles; ++i)
            {
                // Obtain the path of the current file
                string pathName = openFileDialog_stockPick.FileNames[i];
                string ticker = Path.GetFileNameWithoutExtension(pathName);

                //  Creating the form stock viewer
                Form_StockViewer form_StockViewer;
                
                // Setting the first form as the parent form
                if (i == 0)
                {
                    // Reading the file and displaying the stock
                    form_StockViewer = this;
                    readAndDisplayStock();          // Calling the readAndDisplayStock() method to display the stock
                    form_StockViewer.Text = "Parent: " + ticker;        // Calling the stock viewer as the parent
                }
                else
                {
                    // Child form, so instantiating using parameter constructor
                    form_StockViewer = new Form_StockViewer(pathName, startDate, endDate);
                    form_StockViewer.Text = "Child: " + ticker;     // Calling the stock viewer as the child
                }

                // Using the Show() and BringToFront() methods to display
                form_StockViewer.Show();
                form_StockViewer.BringToFront();
            }
        }

        /// <summary>
        /// Opening the file dialog event to allow the user to select the stock file, hence parsing the data to be 
        /// stored in a list, then displaying the candlestick as per our requirement ( data grid view and chart areas)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private List<SmartCandlestick> goReadFile(string filename)
        {
            // Isolating the filename ane extension from the provided path
            this.Text = Path.GetFileName(filename);
            // Creating a reference string for the first line of the user selected file
            const string referenceString = "Date,Open,High,Low,Close,Adj Close,Volume";
            
            // Initializing the list of Candlestick objects
            List<SmartCandlestick> list = new List<SmartCandlestick>();
            // Pass file path and filename to StreamReader constructor
            using (StreamReader sr = new StreamReader(filename))
            {
                // Read first line from new file
                string line = sr.ReadLine();
                // Checking if the first line of the file is the same as the reference string as defined above
                if (line == referenceString)
                {
                    // Reading until there are no more lines to be read
                    while((line = sr.ReadLine()) != null)
                    {
                        SmartCandlestick cs = new SmartCandlestick(line);       // Creating a candlestick object from the read line
                        list.Add(cs);       // Adding the new Candlestick object to the list of Candlesticks
                    }
                    // list.Reverse();      // Specific to Dr Jeanty
                }
                // Case for when a bad file has been opened
                else
                { Text = "Bad File: " + Path.GetFileName(filename); }
            }

            // Iterating through all recognizers in the recognizer dictionary
            foreach (Recognizer r in Dictionary_Recognizer.Values)
            {
                // Add dictionary entries for all patterns on all candlesticks
                r.Recognize_All(list);
            }

            return list;    // returning the list of candlesticks
        }

        /// <summary>
        /// Overload of goReadFile() method
        /// </summary>
        private void goReadFile()
        {
            // Reading data from file into list of candlesticks
            candlesticks = goReadFile(openFileDialog_stockPick.FileName);
            // Binding the list to a Binding List
            boundCandlesticks = new BindingList<SmartCandlestick>(candlesticks);
        }

        /// <summary>
        /// Parsing thorugh candlesticks to filter based on the user specified date range
        /// </summary>
        /// <param name="list">"Candlesticks list containing candlestick objects"</param>
        /// <returns></returns>
        private List<SmartCandlestick> filterList(List<SmartCandlestick> list, DateTime start, DateTime end)
        {
            // Initializing a list of filtered data
            List<SmartCandlestick> filter = new List<SmartCandlestick>(list.Count);
            // Iterating through all the Candlesticks objects in the list to filter
            foreach (SmartCandlestick cs in list) 
            {
                // Condition checking that the date of the candlestick object is within the user specified date range
                if ((cs.date >= start) & (cs.date <= end))
                { filter.Add(cs); }     // Adding the filtered candlestick to the filter list
            }
            return filter;      // returning the filtered list
        }

        /// <summary>
        /// Overload of filterList() method
        /// </summary>
        private void filterList()
        {
            // Calling the filterList() method to filter the SmartCandlesticks list based on the start and end dates
            List<SmartCandlestick> filterCandlesticks = filterList(candlesticks, startDate, endDate);
            // Binding the filtered list to binding list
            boundCandlesticks = new BindingList<SmartCandlestick>(filterCandlesticks);
        }

        /// <summary>
        /// Displaying the bound list of Candlesticks into a data grid view, normalizing the chart, hence displaying
        /// it to the chart areas
        /// </summary>
        /// <param name="bindList">"List of bound candlestickss"</param>
        private void displayCandlesticks(BindingList<SmartCandlestick> bindList)
        {
            // Calling the normalizeChart() method to dyanmically set the Y Axis of the chart to normalize chart size
            normalizeChart();

            // Clearing annotations for the new chart
            chart_OHLCV.Annotations.Clear();

            // Display data by binding list of candlestick objects to chartareas
            chart_OHLCV.DataSource = bindList;
            chart_OHLCV.DataBind();         // data binds the chart control to a data source (https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datavisualization.charting.chart.databind?view=netframework-4.8.1)
        }

        /// <summary>
        /// Overload of displayCandlesticks() method
        /// </summary>
        private void displayCandlesticks()
        {
            // Setting the data source of grid and chart to binding list, hence normalizing the chart
            displayCandlesticks(boundCandlesticks);
        }

        /// <summary>
        /// Normalizing the chart to ensure that the y-axis of the chart is dynamically sized
        /// </summary>
        /// <param name="bindList">"Binding list of Candlesticks objects"</param>
        private void normalizeChart(BindingList<SmartCandlestick> bindList)
        {
            // Set default values for min and max variables
            decimal min = 1000000000, max = 0;
            // Iterate through each candlestick in the binding list
            foreach (SmartCandlestick c in bindList) 
            {
                //Check for greatest value (Ymax) and lowest value (Ymin)
                if (c.low < min) { min = c.low; }       // setting min to the lowest Candlestick.low value
                if (c.high > max) { max = c.high; }     // setting max to the highest Candlestick.high value
            }
            // Setting the Y axis of the chart area to (+-)2% of the ranges rounded to 2 decimal places
            chartMin = chart_OHLCV.ChartAreas["ChartArea_OHLC"].AxisY.Minimum = Math.Floor(Decimal.ToDouble(min) * 0.98);       // -2%
            chartMax = chart_OHLCV.ChartAreas["ChartArea_OHLC"].AxisY.Maximum = Math.Ceiling(Decimal.ToDouble(max) * 1.02);     // +2%
        }

        /// <summary>
        /// Overload of normalizeChart() chart
        /// </summary>
        private void normalizeChart()
        {
            // Finding the range of the candlestick data low/high values, hence setting the chart areas to display dynamically
            normalizeChart(boundCandlesticks);
        }

        /// <summary>
        /// Reads the data from the file, filters the list by dates, and displays data to chart
        /// </summary>
        private void readAndDisplayStock()
        {
            goReadFile();       // Reading file
            filterList();       // Filtering list
            displayCandlesticks();      // Displaying candlestick data
        }

        /// <summary>
        /// Initializing all requuired pattern Recognizer class and stores into a dictionary with pattern name keys
        /// Filling combo box items using dictionary keys
        /// </summary>
        private void InitializeRecognizer()
        {
            Dictionary_Recognizer = new Dictionary<string, Recognizer>();

            // Bullish Recognizer
            Recognizer r = new Recognizer_Bullish();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Bearish Recognizer
            r = new Recognizer_Bearish();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Neutral Recognizer
            r = new Recognizer_Neutral();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Marubozu Recognizer
            r = new Recognizer_Marubozu();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Hammer Recognizer
            r = new Recognizer_Hammer();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Doji Recognizer
            r = new Recognizer_Doji();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Dragonfly Doji Recognizer
            r = new Recognizer_Dragonfly_Doji();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Gravestone Doji Recognizer
            r = new Recognizer_Gravestone_Doji();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Bullish Engulfing Recognizer
            r = new Recognizer_Bullish_Engulfing();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Bearish Engulfing Recognizer
            r = new Recognizer_Bearish_Engulfing();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Bullish Harami Recognizer
            r = new Recognizer_Bullish_Harami();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Bearish Harami Recognizer
            r = new Recognizer_Bearish_Harami();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Peak Recognizer
            r = new Recognizer_Peak();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);
            // Valley Recognizer
            r = new Recognizer_Valley();
            Dictionary_Recognizer.Add(r.Pattern_Name, r);

            // Initializing the Combo Box
            comboBox_Patterns.Items.AddRange(Dictionary_Recognizer.Keys.ToArray());
        }

        /// <summary>
        /// Updating starting date when the user changes the start date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker_startDate_ValueChanged(object sender, EventArgs e)
        {
            // Store starting date in the Form class's startDate attribute
            startDate = dateTimePicker_startDate.Value;
        }

        /// <summary>
        /// Updating ending date when the user changes the end date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker_endDate_ValueChanged(object sender, EventArgs e)
        {
            // Store starting date in the Form class's startDate attribute
            endDate = dateTimePicker_endDate.Value;
        }

        /// <summary>
        /// Event of the combo box to update the chart annotations when an item from the combo box is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Patterns_SelectedIndexChanged(object sender, EventArgs e) 
        {
            // Clearing the annotations for the combo box to change accordingly
            chart_OHLCV.Annotations.Clear();

            if (boundCandlesticks != null)
            {
                // Iterating the bound candlesticks data
                for (int i = 0; i < boundCandlesticks.Count; i++)
                {
                    // Create smart candlestick for the currently indexed boundCandlestick
                    SmartCandlestick scs = boundCandlesticks[i];
                    // Setting the data point from chart 
                    DataPoint point = chart_OHLCV.Series[0].Points[i];

                    string selected = comboBox_Patterns.SelectedItem.ToString();     // Storing the string of combo box pattern
                    // Displaying the current candlestick annotation
                    if (scs.Dictionary_Pattern[selected])
                    {
                        int length = Dictionary_Recognizer[selected].Pattern_Length;        // Store length of pattern
                        // Only annotate for multi-candlestick patterns
                        if (length > 1)
                        {
                            // Edge case for out-of-bound error
                            if (i == 0 | ((i == boundCandlesticks.Count() - 1) & length == 3))
                            {
                                continue;
                            }
                            // Initialize rectangle annotation
                            RectangleAnnotation rectangle = new RectangleAnnotation();
                            rectangle.SetAnchor(point);     // Setting anchor point of the rectangular annotation

                            double Ymax, Ymin;
                            double width = (90.0 / boundCandlesticks.Count()) * length;         // Scalign width of the annotation to the number of candlesticks
                            // Finding Ymax and Ymin between every candlestick pattern
                            if (length == 2)        // Case for even number of candlesticks
                            {
                                Ymax = (int)(Math.Max(scs.high, boundCandlesticks[i-1].high));      // obtaining the maximum for the rectangular annotation
                                Ymin = (int)(Math.Min(scs.low, boundCandlesticks[i-1].low));        // obtaining the minimum for the rectangular annotation
                                rectangle.AnchorOffsetX = ((width / length) / 2 - 0.25) * (-1);        // obtaining the even for the previous candlestick
                            }
                            else       // Case for off number of candlesticks
                            {
                                Ymax = (int)(Math.Max(scs.high, Math.Max(boundCandlesticks[i + 1].high, boundCandlesticks[i - 1].high)));       // obtaining the maximum for the rectangular annotation
                                Ymin = (int)(Math.Min(scs.low, Math.Min(boundCandlesticks[i + 1].low, boundCandlesticks[i - 1].low)));          // obtaining the minimum for the rectangular annotation
                            }
                            double height = 40.0 * (Ymax - Ymin) / (chartMax - chartMin); ; // Scaling to chart bounds
                            rectangle.Height = height; rectangle.Width = width;             // Setting width and height 
                            rectangle.Y = Ymax;                                             // Setting Y of the rectangular annotation to the highest value
                            rectangle.BackColor = Color.Transparent;                        // Making the area transparent so the chart is visible
                            rectangle.LineWidth = 2;                                        // Setting annotation perimeter width to 2
                            rectangle.LineDashStyle = ChartDashStyle.Dash;                  // Making the perimeter to be dashed
                            // Adding the annotation to the chart
                            chart_OHLCV.Annotations.Add(rectangle);
                        }

                        // Initialize arrow annotation
                        ArrowAnnotation arrow = new ArrowAnnotation();
                        // Setting the properties for the arrow annotations
                        arrow.AxisX = chart_OHLCV.ChartAreas[0].AxisX;      // Setting the arrow annotation's X axis
                        arrow.AxisY = chart_OHLCV.ChartAreas[0].AxisY;      // Setting the arrow annotation's Y axis
                        arrow.Width = 0.5;      // Setting the arrow annotation's width
                        arrow.Height = 0.5;     // Setting the arrow annotation's height
                        // Annotating the single pattern and main candlestick for all multi-candlesticks
                        arrow.SetAnchor(point);
                        chart_OHLCV.Annotations.Add(arrow);     // adding the arrow to the chart annotation
                    }
                }
            }
        }

        private void Form_Project_2_Load(object sender, EventArgs e)
        {

        }
    }
}