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

        public static void PrintDataTable(String filePath)
        {
            String line;
            try
            {
                // Create a StreamReader object to read the file
                StreamReader sr = new(filePath);
                //Read the first line of text
                line = sr.ReadLine();
                //Check if the line is not null (end of file)
                if (line == null)
                {
                    PrintMessage("File is empty");
                    return;
                }

                // Split headers
                Console.WriteLine(line);
                String[] headers = line.Split(",");

                if (headers == null || headers.Length == 0)
                {
                    PrintMessage("No headers found in file");
                    return;
                }
                // Print headers in table format
                int tableWidth = GUI_WIDTH - PADDING;
                int tableHeaderWidth = tableWidth / headers.Length;
                string tableHeader = "+" + new string(' ', 2);
                foreach (var header in headers)
                {
                    tableHeader += "|  " + header + "  ";
                }
                tableHeader += "  |" + new string(' ', 2) + "+";
                Console.WriteLine($"|{new string(' ', PADDING/2)}{new string('+', tableHeader.Length)}{new string(' ', PADDING/2)}|");                 
                Console.WriteLine($"|{new string(' ', PADDING/2)}{tableHeader}{new string(' ', PADDING/2)}|");
                Console.WriteLine($"|{new string(' ', PADDING/2)}{new string('+', tableHeader.Length)}{new string(' ', PADDING/2)}|");

                // Each row of the text file data (i.e., one cell tower record)
                List<String[]> records = [];

                //Continue to read until you reach end of file
                while (line != null)
                {
                    //capture data in the line
                    String[] data = line.Split(",");
                    //check if the data is not null (end of file)
                    if (data == null)
                    {
                        PrintMessage("End of file reached");
                        break;
                    }

                    // add the data to the records list
                    records.Add(data);

                    // print row of data in table format
                    // Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
                    // Console.WriteLine($"|{new string(' ', PADDING)}{string.Join(new string(' ', headerWidth), data)}{new string(' ', PADDING)}|");
                    // Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");                    

                    //Read the next line
                    line = sr.ReadLine();                    
                }
                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch(Exception e)
            {
                PrintMessage($"Error when printing the data table. Ensure that the file exists and is not empty.");
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            string title = "Cell Tower Frequency Distributer Application";
            int padding = 5;
            PrintTerminalHeader();

            // file path to Txt file
            string filePath = "./data/celltowerData.txt";

            // show data in table format
            PrintDataTable(filePath);




            PrintTerminalFooter();
        }
    }
}