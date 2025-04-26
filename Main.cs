using System;

namespace CellTowerFrequencies
{
    class MainClass
    {
        public static string TITLE { get { return "Cell Tower Frequency Distributer Application"; } }
        public static int PADDING { get { return 15; } }
        public static int GUI_WIDTH { get { return TITLE.Length + (PADDING * 2); } }

        static void PrintTerminalHeader()
        {
            // Console.Clear();
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string(' ', PADDING)}{TITLE}{new string(' ', PADDING)}|");
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
        }

        public static void PrintTerminalFooter()
        {
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
        }

        public static void PrintMessage(string message) // prints a message in the center of the GUI
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "No message provided";
            }

            int contentWidth = GUI_WIDTH - 2 * PADDING;

            // Break the message into lines based on the content width
            List<string> lines = new List<string>();
            while (message.Length > contentWidth)
            {
                // Find last space within the limit for a clean break
                int breakIndex = message.LastIndexOf(' ', contentWidth);
                if (breakIndex <= 0) breakIndex = contentWidth;

                lines.Add(message.Substring(0, breakIndex));
                message = message.Substring(breakIndex).TrimStart();
            }

            lines.Add(message); // Add any remaining text

            // Print the top border
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");

            // Print each line centered within the GUI
            foreach (var line in lines)
            {
                int paddingRight = contentWidth - line.Length;
                Console.WriteLine($"|{new string(' ', PADDING)}{line}{new string(' ', paddingRight + PADDING)}|");
            }

            // Print the bottom border
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
        }

        public static List<CellTower> PrintDataTable(string filePath)
        {
            string line;
            int tableWidth = GUI_WIDTH - PADDING;
            int tableHeaderWidth = 0;
            string tableHeader = "+";
            // Each row of the text file data (i.e., one cell tower record)
            List<CellTower> records = [];

            try
            {
                PrintMessage("Reading data from file...");

                // Create a StreamReader object to read the file
                StreamReader sr = new(filePath);

                //Read the first line of text
                line = sr.ReadLine();

                //Check if the line is not null (end of file)
                if (line == null)
                {
                    return [];
                }

                // Split headers
                string[] headers = line.Split(",");

                if (headers == null || headers.Length == 0)
                {
                    return [];
                }

                // build headers in table format
                tableHeaderWidth = tableWidth / headers.Length;
                foreach (var header in headers)
                {
                    tableHeader += new string(' ', 2) + header + "  |";
                }
                tableHeader += "+";

                // Read the next line - this will be the first record of data
                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    //capture data in the line
                    string[] data = line.Split(",");

                    // check if we reached the end of the file
                    if (data == null)
                    {
                        break;
                    }

                    // check if the data contains the correct number of columns
                    if (data.Length != headers.Length)
                    {
                        PrintMessage($"Incorrect number of columns in line: {line}. Skipping Cell: " + data[0]);
                        break;
                    }

                    // add the data to the records list
                    CellTower cellTower = new CellTower(id:data[0], easting:double.Parse(data[1]), northing:double.Parse(data[2]), longitude:double.Parse(data[3]), latitude:double.Parse(data[4]));
                    records.Add(cellTower);

                    // print row of data in table format
                    // Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
                    // Console.WriteLine($"|{new string(' ', PADDING)}{string.Join(new string(' ', headerWidth), data)}{new string(' ', PADDING)}|");
                    // Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");                    

                    //Read the next line
                    line = sr.ReadLine();                    
                }
                //close the file
                sr.Close();
                
                // Show Table
                // --- Header ---
                // Console.WriteLine($"|{new string(' ', PADDING/2)}{new string('+', tableHeader.Length)}{new string(' ', PADDING/2)}|");                 
                // Console.WriteLine($"|{new string(' ', PADDING/2)}{tableHeader}{new string(' ', PADDING/2)}|");
                // Console.WriteLine($"|{new string(' ', PADDING/2)}{new string('+', tableHeader.Length)}{new string(' ', PADDING/2)}|");
                // // --- Data ---
                // foreach (var record in records)
                // {
                //     string row = "|";
                //     for (int i=0; i<record.Length; i++)
                //     {
                //         row += new string(' ', PADDING/2) + "+" + new string(' ', 2) + $"{record[i], }" + new string(' ', 2) + "|";
                //     }
                //     Console.WriteLine($"|{new string(' ', PADDING/2)}{row}{new string(' ', PADDING/2)}|");
                // }

                // Console.WriteLine($"|{new string(' ', PADDING/2)}{new string('+', tableHeader.Length)}{new string(' ', PADDING/2)}|");

                return records;
            }
            catch(Exception e)
            {
                PrintMessage($"Error when printing the data table. Ensure that the file exists and is not empty.");
                Console.WriteLine(e.Message);
                return [];
            }
        }

        static void Main(string[] args)
        {
            // ----------------------------------------------------------------------------------------------------------------------
            // Distance Thereshold: We make the assumption that the minimum distance to classify a cell tower as "close" is 500m.
            // ----------------------------------------------------------------------------------------------------------------------
            int closeThreshold = 500; // meters

            PrintTerminalHeader();

            // file path to Txt file
            string filePath = "./data/celltowerData.txt";

            // show data in table format
            List<CellTower> cellTowers = PrintDataTable(filePath);

            if (cellTowers == null || cellTowers.Count == 0)
            {
                PrintMessage("No data found in file. Exiting application.");
                return;
            }

            for (int i = 0; i < cellTowers.Count; i++)
            {
                // add neighbouring or out of range cell towers to each cell tower
                for (int j = 0; j < cellTowers.Count; j++)
                {
                    if (i != j)
                    {
                        cellTowers[i].AddCellTower(cellTowers[j], closeThreshold);
                    }
                }

                // Print the cell tower details
                Console.WriteLine($"Cell Tower ID: {cellTowers[i].Id}");
                Console.WriteLine($"Latitude: {cellTowers[i].Latitude}");
                Console.WriteLine($"Longitude: {cellTowers[i].Longitude}");
                Console.WriteLine($"Northing: {cellTowers[i].Northing}");
                Console.WriteLine($"Easting: {cellTowers[i].Easting}");
                Console.WriteLine($"Frequency: {cellTowers[i].frequency}");
                Console.WriteLine($"Nearby Cell Towers: {string.Join(", ", cellTowers[i].NearbyCellTowers.Select(t => t.Item1.Id))}");
                Console.WriteLine($"Out of Range Cell Towers: {string.Join(", ", cellTowers[i].OutOfRangeCellTowers.Select(t => t.Item1.Id))}");

            }

            // Print the footer
            PrintTerminalFooter();
        }
    }
}