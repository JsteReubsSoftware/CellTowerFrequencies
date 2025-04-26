using System;

namespace CellTowerFrequencies
{
    class MainClass
    {
        public static string TITLE { get { return "Cell Tower Frequency Distributer Application"; } }
        public static int PADDING { get { return 15; } }
        public static int GUI_WIDTH { get { return 10 + TITLE.Length + (PADDING * 2); } }
        public static int MENU_WIDTH { get { return GUI_WIDTH - 2 * PADDING; } }
        public static Dictionary<string, string> CONFIG { get; set;}

        static void PrintTerminalHeader()
        {
            // Console.Clear();
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string(' ', PADDING)}{TITLE}{new string(' ', GUI_WIDTH - (PADDING+TITLE.Length))}|");
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
        }

        public static void PrintTerminalFooter()
        {
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");
            Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
        }

        public static void PrintMessage(string message, bool addBottomBorder = true) // prints a message in the center of the GUI
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
            if (addBottomBorder)
            {
                Console.WriteLine($"|{new string('-', GUI_WIDTH)}|");
            }
            else
            {
                Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");
            }
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

        public static void PrintMenu()
        {
            string msg = "Please select an option from the menu below:";
            string[] options = {
                "Update Configuration",
                "Run Application",
                "View data",
                "Exit Application"
            };
            
            string output = "";
            for (int i=0; i<options.Length; i++)
            {
                string enterText = $"{$"- Enter '{i+1}'",10}";
                string innerText = $"{i+1}.{options[i],-35}{enterText}";
                output += "|"+ new string(' ', PADDING/2) + innerText + new string(' ', GUI_WIDTH - (PADDING / 2 + innerText.Length)) + "|\n";
            }
            output += "|"+ new string(' ', GUI_WIDTH) + "|";

            Console.WriteLine("|"+ new string(' ', GUI_WIDTH) + "|\n" +
                            "|"+ new string(' ', PADDING / 2) + msg + new string(' ', GUI_WIDTH - (PADDING / 2 + msg.Length)) + "|");
            Console.WriteLine(output);
        }

        public static void PrintConfiguration(bool updatingConfig = false)
        {
            string msg = "Application Configuration:";
            string[] options = [ // if you update these please also update the CONFIG variable in the first part of the Main method
                "File Path",
                "Distance Threshold",
                "Frequency Range",
                "Number of Frequencies"
            ];

            if (updatingConfig)
            {
                options = [options[0], options[1], "<- Back", "<- Exit Application"]; // we exclude the option to change frequencies. The specification document states to use the default values.
            }

            string output = "";
            for (int i=0; i<options.Length; i++)
            {
                string innerText = $"{i+1}.{options[i],-35}";
                innerText += updatingConfig ? $"{$"- Enter '{i+1}'",10}" : $"{CONFIG[options[i]],30}";
                
                output += "|"+ new string(' ', PADDING/2) + innerText + new string(' ', GUI_WIDTH - (PADDING / 2 + innerText.Length)) + "|\n";
            }

            output += "|"+ new string(' ', GUI_WIDTH) + "|";

            Console.WriteLine("|"+ new string(' ', PADDING / 2) + msg + new string(' ', GUI_WIDTH - (PADDING / 2 + msg.Length)) + "|");
            Console.WriteLine(output);
        }


        public static void UpdateConfiguration(string key, string value)
        {
            if (CONFIG.ContainsKey(key))
            {
                CONFIG[key] = value;
            }
            else
            {
                PrintMessage($"Invalid key: {key}. Please try again.");
            }
        }

        public static string PromptUser(string msg = "Please enter your choice: ", bool optionSelection = true)
        {
            Console.Write("|"+ new string(' ', PADDING / 2) + msg);
            string choice = Console.ReadLine();
            return choice;
        }

        static void Main(string[] args)
        {
            // ----------------------------------------------------------------------------------------------------------------------
            // Distance Thereshold: We make the assumption that the minimum distance to classify a cell tower as "close" is 500m.
            // ----------------------------------------------------------------------------------------------------------------------
            // default values
            string filePath = "./data/celltowerData.txt"; // path to the data file
            int closeThreshold = 500; // meters
            int numFrequencies = 6; // number of frequencies that can be used
            int[] freqRange = new int[numFrequencies]; // range of frequencies that can be used
            int startFreq = 110; // starting frequency

            for (int i = 0; i < numFrequencies; i++)
            {
                freqRange[i] = startFreq + i; // define the range of frequencies
            }

            // Set the configuration values
            CONFIG = new Dictionary<string, string>();
            CONFIG["File Path"] = filePath;
            CONFIG["Distance Threshold"] = closeThreshold.ToString();
            CONFIG["Frequency Range"] = string.Join(", ", freqRange);
            CONFIG["Number of Frequencies"] = numFrequencies.ToString();

            PrintTerminalHeader();
            PrintMessage(new string(' ', PADDING) + "Welcome!", addBottomBorder: false);
            PrintConfiguration();
            while (true)
            {
                PrintMenu();
                
                // Prompt user for input
                string choice = PromptUser();

                // process option
                switch(choice)
                {
                    case "1":
                        // Update Configuration
                        bool validInput = false;
                        while (!validInput)
                        {
                            PrintConfiguration(updatingConfig: true);
                            choice = PromptUser();

                            switch (choice)
                            {
                                case "1":
                                    choice = PromptUser("Please enter the new file path: ");

                                    if (string.IsNullOrWhiteSpace(choice) || !File.Exists(choice))
                                    {
                                        PrintMessage($"Invalid file path. The path: '{choice}' does not exist. Please try again. By selecting which option to update.");
                                        break;
                                    }
                                    UpdateConfiguration("FilePath", choice);  
                                    // maybe print updated configuration

                                    validInput = true;
                                    break;
                                case "2":
                                    choice = PromptUser("Please enter the new distance threshold (in meters): ");
                                    if (string.IsNullOrWhiteSpace(choice) || int.Parse(choice) <= 0)
                                    {
                                        PrintMessage($"Invalid distance threshold. Please try again. By selecting which option to update.");
                                        break;
                                    }
                                    UpdateConfiguration("DistThreshold", choice);
                                    validInput = true;
                                    break;
                                case "3":
                                    validInput = true;
                                    break;
                                case "4":
                                    PrintMessage("Exiting application...");
                                    return;
                                default:
                                    PrintMessage($"Invalid option. Please try again. By selecting which option to update.");
                                    break;
                            }
                        }
                        

                        break;
                    case "2":
                        // Run Application
                        PrintMessage("Running application...");
                        break;
                    case "3":
                        // View Data
                        PrintMessage("Viewing data...");
                        break;
                    case "4":
                        // Exit Application
                        PrintMessage("Exiting application...");
                        return;
                }
            }

            // // Print the header
            // PrintTerminalHeader();

            // // show data in table format and return the list of cell towers
            // List<CellTower> cellTowers = PrintDataTable(filePath);

            // if (cellTowers == null || cellTowers.Count == 0)
            // {
            //     PrintMessage("No data found in file. Exiting application.");
            //     return;
            // }

            // for (int i = 0; i < cellTowers.Count; i++)
            // {
            //     // add neighbouring or out of range cell towers to each cell tower
            //     for (int j = 0; j < cellTowers.Count; j++)
            //     {
            //         if (i != j)
            //         {
            //             cellTowers[i].AddCellTower(cellTowers[j], closeThreshold);
            //         }
            //     }
            // }

            // // Run the Graph Coloring Algorithm to assign frequencies to the cell towers
            // GraphColoringAlgorithm gca = new GraphColoringAlgorithm()

            // // Print the footer
            // PrintTerminalFooter();
        }
    }
}