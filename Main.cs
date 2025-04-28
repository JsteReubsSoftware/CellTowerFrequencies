using System;
using System.Data;

namespace CellTowerFrequencies
{
    class MainClass
    {
        public static string TITLE { get { return "Cell Tower Frequency Distributer Application"; } }
        public static int PADDING { get { return 15; } }
        public static int GUI_WIDTH { get { return 10 + TITLE.Length + (PADDING * 2); } }
        public static string[] FILE_HEADERS { get; set; } // headers of the data file
        public static List<CellTower> CELL_TOWER_DATA { get; set; } // list of cell towers
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

        public static void BuildCellTowers()
        {
            string line;
            // Each row of the text file data (i.e., one cell tower record)
            List<CellTower> records = [];

            try
            {
                // Create a StreamReader object to read the file
                StreamReader sr = new StreamReader(CONFIG["File Path"]) ?? throw new Exception("File not found.");

                //Read the first line of text
                line = sr.ReadLine() ?? string.Empty;

                //Check if the line is not null (end of file)
                if (line == null || line.Length == 0)
                {
                    throw new Exception($"Error when reading the file. Ensure that the file contains data.");
                }

                // Split headers
                string[] headers = line.Split(",");

                if (headers == null || headers.Length == 0)
                {
                    throw new Exception($"Error when building the cell towers. The file contains no headers.");
                }

                // Read the next line - this will be the first record of data
                line = sr.ReadLine() ?? string.Empty;

                //Continue to read until you reach end of file
                while (line != null && line.Length > 0)
                {
                    //capture data in the line
                    string[] data = line.Split(",");

                    // check if we reached the end of the file
                    if (data == null || data.Length == 0)
                    {
                        break;
                    }

                    // check if the data contains the correct number of columns
                    if (data.Length != headers.Length)
                    {
                        PrintMessage($"Incorrect number of columns in line: {line}. Skipping Cell: " + data[0], addBottomBorder: false);
                        //Read the next line
                        line = sr.ReadLine() ?? string.Empty;
                        continue;
                    }

                    // add the data to the records list
                    CellTower cellTower = new CellTower(id:data[0], easting:double.Parse(data[1]), northing:double.Parse(data[2]), longitude:double.Parse(data[3]), latitude:double.Parse(data[4]));
                    records.Add(cellTower);                    

                    //Read the next line
                    line = sr.ReadLine() ?? string.Empty;                    
                }
                //close the file
                sr.Close();

                // Update global variables
                CELL_TOWER_DATA = records;
                FILE_HEADERS = headers;

                PrintMessage($"Cell tower data built successfully. {records.Count} records found.");
            }
            catch(Exception e)
            {
                PrintMessage($"Error building cell tower data.");
                Console.WriteLine(e.Message);
            }
        }

        public static void PrintDataTable(string filePath)
        {
            try
            {
                // Check if data has been built
                if (CELL_TOWER_DATA == null) {
                    PrintMessage($"Cell tower data not built. Building data from file: {filePath}.");
                    BuildCellTowers();
                }
                
                if (CELL_TOWER_DATA == null || CELL_TOWER_DATA.Count == 0)
                {
                    throw new Exception($"Error when printing the data table. The file contains no data.");
                }

                // check if file headers are valid
                if (FILE_HEADERS == null || FILE_HEADERS.Length == 0)
                {
                    throw new Exception($"Error when printing the data table. The file contains no headers.");
                }

                // build headers in table format
                int tableWidth = GUI_WIDTH - PADDING;
                int numCols = FILE_HEADERS.Length;
                int colWidth = FILE_HEADERS.Select(x => x.Length).Max() + 4; // add 2 for padding
                colWidth = Math.Max(colWidth, 10); // minimum column width
                string tableHeader = $"|{new string(' ', PADDING/2)}";
                string headingBorder = "+";
                string tableData = "";
                string tableHeaderNames = "|";

                // build header border
                for (int c=0; c<numCols; c++)
                {
                    headingBorder += $"{new string('-', colWidth)}+";
                }
                tableHeader += headingBorder + $"{new string(' ', GUI_WIDTH - (PADDING/2 + headingBorder.Length))}|\n";

                // build header names
                for (int h=0; h<FILE_HEADERS.Length; h++)
                {
                    tableHeaderNames += $"{new string(' ', 2)}{FILE_HEADERS[h]}{new string(' ', colWidth - (2 + FILE_HEADERS[h].Length))}|";
                }
                tableHeader += $"|{new string(' ', PADDING/2)}{tableHeaderNames}{new string(' ', GUI_WIDTH - (PADDING / 2 + tableHeaderNames.Length))}|\n";
                tableHeader += $"|{new string(' ', PADDING/2)}{headingBorder}{new string(' ', GUI_WIDTH - (PADDING / 2 + headingBorder.Length))}|\n";

                // Print table content
                for (int i=0; i<CELL_TOWER_DATA.Count; i++)
                {
                    string row = $"|{new string(' ', PADDING/2)}|{new string(' ', 2)}{CELL_TOWER_DATA[i].Id}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Id.Length))}|"+
                                $"{new string(' ', 2)}{CELL_TOWER_DATA[i].Easting}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Easting.ToString().Length))}|" +
                                $"{new string(' ', 2)}{CELL_TOWER_DATA[i].Northing}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Northing.ToString().Length))}|" +
                                $"{new string(' ', 2)}{CELL_TOWER_DATA[i].Longitude}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Longitude.ToString().Length))}|" +
                                $"{new string(' ', 2)}{CELL_TOWER_DATA[i].Latitude}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Latitude.ToString().Length))}|";
                    tableData += row + new string(' ', GUI_WIDTH - row.Length + 1) + "|\n";
                }
                tableData += $"|{new string(' ', PADDING/2)}{headingBorder}{new string(' ', GUI_WIDTH - (PADDING / 2 + headingBorder.Length))}|\n";
                
                Console.WriteLine(tableHeader + tableData + "|"+ new string(' ', GUI_WIDTH) + "|");
            }
            catch(Exception e)
            {
                PrintMessage($"Error when printing the data table. Ensure that the file exists and is not empty.");
                Console.WriteLine(e.Message);
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
            string msg = "Select property to update:";
            string[] options = [ // if you update these please also update the CONFIG variable in the first part of the Main method
                "File Path",
                "Distance Threshold (m)",
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

            Console.WriteLine("|"+ new string(' ', GUI_WIDTH) + "|\n" +
                            "|"+ new string(' ', PADDING / 2) + msg + new string(' ', GUI_WIDTH - (PADDING / 2 + msg.Length)) + "|");
            Console.WriteLine(output);
        }


        public static void UpdateConfiguration(string key, string value)
        {
            if (CONFIG.ContainsKey(key))
            {
                CONFIG[key] = value;
                PrintMessage($"Configuration updated: {key} = {value}");
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
            CONFIG["Distance Threshold (m)"] = closeThreshold.ToString();
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
                                    UpdateConfiguration("File Path", choice);  
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
                                    UpdateConfiguration("Distance Threshold (m)", choice);
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

                        // build cell towers from the data file
                        if (CELL_TOWER_DATA == null || CELL_TOWER_DATA.Count == 0)
                        {                    
                            PrintMessage($"Cell tower data not built. Building data from file: {CONFIG["File Path"]}.");        
                            BuildCellTowers();
                        }

                        if (CELL_TOWER_DATA == null || CELL_TOWER_DATA.Count == 0)
                        {
                            PrintMessage("No cell tower data found. Exiting application.");
                            return;
                        }

                        GraphColoringAlgorithm gca = new GraphColoringAlgorithm(freqRange);
                        List<(int, int)> frequencyCount = gca.RunGCA(CELL_TOWER_DATA, 42);

                        if (frequencyCount == null || frequencyCount.Count == 0)
                        {
                            PrintMessage("Error running the graph coloring algorithm. Exiting application.");
                            return;
                        }

                        // Print the frequency count
                        string[] freqTableHeaders = ["Frequency (MHz)", "Count"];
                        int colWidth = freqTableHeaders.Select(x => x.Length).Max() + 4; // add 2 for padding
                        colWidth = Math.Max(colWidth, 10); // minimum column width
                        string borderTopBottom = $"+{new string('-', colWidth)}+{new string('-', colWidth)}+";
                        
                        // create header
                        string freqTblHeader = $"|{new string(' ', PADDING / 2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|\n";
                        string freqTblHeaderNames = $"|";
                        for (int i=0; i<freqTableHeaders.Length; i++)
                        {
                            freqTblHeaderNames += $"{new string(' ', 2)}{freqTableHeaders[i]}{new string(' ', colWidth - (2 + freqTableHeaders[i].Length))}|";
                        }
                        freqTblHeader += $"|{new string(' ', PADDING/2)}{freqTblHeaderNames}{new string(' ', GUI_WIDTH - (PADDING / 2 + freqTblHeaderNames.Length))}|\n";
                        freqTblHeader += $"|{new string(' ', PADDING/2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|\n";

                        // create footer
                        string footerMsg = $"|{new string(' ', 2)}Total Frequencies Used: {frequencyCount.Sum(x => x.Item2)}/{numFrequencies}";
                        footerMsg += $"{new string(' ', borderTopBottom.Length - footerMsg.Length - 1)}|";
                        
                        string freqTblFooter = $"|{new string(' ', PADDING/2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|\n";
                        freqTblFooter += $"|{new string(' ', PADDING/2)}{footerMsg}{new string(' ', GUI_WIDTH - (PADDING / 2 + footerMsg.Length))}|\n";
                        freqTblFooter += $"|{new string(' ', PADDING/2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|";

                        // create data
                        string freqTblData = "";
                        for (int i = 0; i < frequencyCount.Count; i++)
                        {
                            string row = $"|{new string(' ', PADDING/2)}|{new string(' ', 2)}{frequencyCount[i].Item1}{new string(' ', colWidth - (2 + frequencyCount[i].Item1.ToString().Length))}|"+
                                $"{new string(' ', 2)}{frequencyCount[i].Item2}{new string(' ', colWidth - (2 + frequencyCount[i].Item2.ToString().Length))}|";
                            freqTblData += row + new string(' ', GUI_WIDTH - row.Length + 1) + "|\n";
                        }
                        freqTblData += $"|{new string(' ', PADDING/2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|\n";

                        string msg = $"Successfully ran application. Frequencies assigned to cell towers:";
                        Console.WriteLine($"|{new string(' ', PADDING /2)}{msg}{new string (' ', GUI_WIDTH - (PADDING / 2 + msg.Length))}|");
                        Console.WriteLine(freqTblHeader + freqTblData + freqTblFooter);
                        Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");
                        break;
                    case "3":
                        // View Data
                        PrintMessage("Viewing data...");
                        PrintDataTable(CONFIG["File Path"]);
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