using System;
using System.Data;

namespace CellTowerFrequencies
{
    class MainClass
    {
        public static string TITLE { get { return "Cell Tower Frequency Distributer Application"; } }
        public static int PADDING { get { return 15; } }
        public static int GUI_WIDTH { get { return Math.Max(140, 10 + TITLE.Length + (PADDING * 2)); } }
        public static string[] FILE_HEADERS { get; set; } // headers of the data file
        public static List<CellTower> CELL_TOWER_DATA { get; set; } // list of cell towers
        public static Dictionary<string, string> CONFIG { get; set;}
        public static int MAX_LENGTH_NEARBY { get; set; } = 0; // maximum length of the nearby cell towers
        public static int MAX_LENGTH_OUT_OF_RANGE { get; set; } = 0; // maximum length of the out of range cell towers
        public static List<(string, string, string)> OTHER_TOWERS { get; set; }// list of other towers
        public static bool UPDATED_CONFIG { get; set; } = false; // flag to check if the configuration has been updated

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

                // add cell towers and set max lengths
                OTHER_TOWERS = new List<(string, string, string)>(); // reset the other towers list
                MAX_LENGTH_NEARBY = 0; // reset the max length of the nearby cell towers
                MAX_LENGTH_OUT_OF_RANGE = 0; // reset the max length of the out of range cell towers
                
                double threshold = double.Parse(CONFIG["Distance Threshold (m)"]);
                for (int i = 0; i < CELL_TOWER_DATA.Count; i++)
                {
                    // add neighbouring or out of range cell towers to each cell tower
                    for (int j = 0; j < CELL_TOWER_DATA.Count; j++)
                    {
                        bool containsNearby = CELL_TOWER_DATA[i].NearbyCellTowers.Any(t => t.Item1 == CELL_TOWER_DATA[j]);
                        bool containsOutOfRange = CELL_TOWER_DATA[i].OutOfRangeCellTowers.Any(t => t.Item1 == CELL_TOWER_DATA[j]);

                        if (i != j && !containsNearby && !containsOutOfRange)
                        {
                            CELL_TOWER_DATA[i].AddCellTower(CELL_TOWER_DATA[j], threshold);
                        }
                    }
                    OTHER_TOWERS.Add((CELL_TOWER_DATA[i].Id, string.Join(",", CELL_TOWER_DATA[i].NearbyCellTowers.Select(x => x.Item1.Id)), string.Join(",", CELL_TOWER_DATA[i].OutOfRangeCellTowers.Select(x => x.Item1.Id))));
                    
                    if (OTHER_TOWERS[i].Item2.Length+4 > MAX_LENGTH_NEARBY)
                    {
                        MAX_LENGTH_NEARBY = OTHER_TOWERS[i].Item2.Length + 4;
                    }
                    if (OTHER_TOWERS[i].Item3.Length+4 > MAX_LENGTH_OUT_OF_RANGE)
                    {
                        MAX_LENGTH_OUT_OF_RANGE = OTHER_TOWERS[i].Item3.Length + 4;
                    }
                }

                PrintMessage($"Cell tower data built successfully. {records.Count} records found.");
            }
            catch(Exception e)
            {
                PrintMessage($"Error building cell tower data.");
                Console.WriteLine(e.Message);
            }
        }

        public static (string, string, int, int) BuildTableHeader(string[] headers, bool showingList = false, int maxColWidth = 0)
        {// returns (tableHeader, headingBorder, numCols, idColWidth, otherTowers, maxLengthNearby, maxLengthOutOfRange)
            int numCols = headers.Length;
            int colWidth = headers.Select(x => x.Length).Max() + 4; // add 2 for padding
            colWidth = Math.Max(colWidth, 10); // minimum column width
            string tableHeader = $"|{new string(' ', PADDING/2)}";
            string headingBorder = "+";
            string tableHeaderNames = "|";

            // build header border
            for (int c=0; c<numCols; c++)
            {
                if (showingList && c == 1) // if showing list, we need to add the max length of the nearby towers
                {
                    headingBorder += $"{new string('-', (MAX_LENGTH_NEARBY > colWidth ? MAX_LENGTH_NEARBY : colWidth))}+";
                }
                else if (showingList && c == 2) // if showing list, we need to add the max length of the out of range towers
                {
                    headingBorder += $"{new string('-', (MAX_LENGTH_OUT_OF_RANGE > colWidth ? MAX_LENGTH_OUT_OF_RANGE : colWidth))}+";
                }
                else if (maxColWidth > 0 && c == 1) // if showing list, we need to add the max length of the nearby towers
                {
                    headingBorder += $"{new string('-',  (maxColWidth > colWidth ? maxColWidth : colWidth))}+";
                }
                else
                {
                    headingBorder += $"{new string('-', colWidth)}+";
                }
            }
            tableHeader += headingBorder + $"{new string(' ', GUI_WIDTH - (PADDING/2 + headingBorder.Length))}|\n";

            // build header names
            for (int h=0; h<numCols; h++)
            {
                if (showingList && h == 1) // if showing list, we need to add the max length of the nearby towers
                {
                    tableHeaderNames += $"{new string(' ', 2)}{headers[h]}{new string(' ', (MAX_LENGTH_NEARBY > colWidth ? MAX_LENGTH_NEARBY : colWidth) - (2 + headers[h].Length))}|";
                }
                else if (showingList && h == 2) // if showing list, we need to add the max length of the out of range towers
                {
                    tableHeaderNames += $"{new string(' ', 2)}{headers[h]}{new string(' ', (MAX_LENGTH_OUT_OF_RANGE > colWidth ? MAX_LENGTH_OUT_OF_RANGE : colWidth) - (2 + headers[h].Length))}|";
                }
                else if (maxColWidth > 0 && h == 1) // if showing list, we need to add the max length of the nearby towers
                {
                    tableHeaderNames += $"{new string(' ', 2)}{headers[h]}{new string(' ', (maxColWidth > colWidth ? maxColWidth : colWidth) - (2 + headers[h].Length))}|";
                }
                else
                {
                    tableHeaderNames += $"{new string(' ', 2)}{headers[h]}{new string(' ', colWidth - (2 + headers[h].Length))}|";
                }
                
            }
            tableHeader += $"|{new string(' ', PADDING/2)}{tableHeaderNames}{new string(' ', GUI_WIDTH - (PADDING / 2 + tableHeaderNames.Length))}|\n";
            tableHeader += $"|{new string(' ', PADDING/2)}{headingBorder}{new string(' ', GUI_WIDTH - (PADDING / 2 + headingBorder.Length))}|\n";

            return (tableHeader, headingBorder, numCols, colWidth);
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

                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //                      BUILD A TABLE FOR SHOWING TOWER COORDINATES
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                // build headers in table format
                (string, string, int, int) tupleHeader = BuildTableHeader(FILE_HEADERS);
                string tableHeader = tupleHeader.Item1;
                string headingBorder = tupleHeader.Item2;
                int numCols = tupleHeader.Item3;
                int colWidth = tupleHeader.Item4;
                string tableData = "";                

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
                
                string cellTowerDistData = tableHeader + tableData + "|"+ new string(' ', GUI_WIDTH) + "|";

                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //                      BUILD A TABLE FOR SHOWING NEARBY TOWERS & OUT OF RANGE TOWERS
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                string[] tableHeaders = ["Cell Tower", "Nearby", "Out of Range"];
                (string, string, int, int) tupleHeader2 = BuildTableHeader(tableHeaders, showingList: true);
                tableHeader = tupleHeader2.Item1;
                headingBorder = tupleHeader2.Item2;
                numCols = tupleHeader2.Item3;
                colWidth = tupleHeader2.Item4;
                tableData = "";

                // Print table content
                for (int i=0; i<CELL_TOWER_DATA.Count; i++)
                {
                    string row = $"|{new string(' ', PADDING/2)}|{new string(' ', 2)}{CELL_TOWER_DATA[i].Id}{new string(' ', colWidth - (2 + CELL_TOWER_DATA[i].Id.Length))}|"+
                                $"{new string(' ', 2)}{OTHER_TOWERS[i].Item2}{new string(' ', (MAX_LENGTH_NEARBY > colWidth ? MAX_LENGTH_NEARBY : colWidth) - (2 + OTHER_TOWERS[i].Item2.Length))}|" +
                                $"{new string(' ', 2)}{OTHER_TOWERS[i].Item3}{new string(' ', (MAX_LENGTH_OUT_OF_RANGE > colWidth ? MAX_LENGTH_OUT_OF_RANGE : colWidth) - (2 + OTHER_TOWERS[i].Item3.Length))}|";
                    tableData += row + new string(' ', GUI_WIDTH - row.Length + 1) + "|\n";
                }
                int requiredWidth = GUI_WIDTH - (PADDING / 2 + headingBorder.Length);
                tableData += $"|{new string(' ', PADDING/2)}{headingBorder}{new string(' ', (requiredWidth > 0 ? requiredWidth : 0))}|\n";

                string cellTowerNearbyOutOfRangeData = tableHeader + tableData + "|"+ new string(' ', GUI_WIDTH) + "|";
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //                                              PRINT CONTENT
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                Console.WriteLine(cellTowerDistData);
                Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");
                Console.WriteLine(cellTowerNearbyOutOfRangeData);
                Console.WriteLine($"|{new string(' ', GUI_WIDTH)}|");

            }
            catch(Exception e)
            {
                PrintMessage($"Error when printing the data table.");
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
            int seed = 42; // seed for random number generation
            // set global seed
            Random rand = new Random(seed);

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
                        UPDATED_CONFIG = true; // set the flag to true to indicate that the configuration has been updated  
                        OTHER_TOWERS = new List<(string, string, string)>(); // reset the other towers list
                        CELL_TOWER_DATA = new List<CellTower>(); // reset the cell tower data                      

                        break;
                    case "2":
                        // Run Application
                        PrintMessage("Running application...");

                        // if the configuration has been updated, we need to update the cell tower data
                        if (UPDATED_CONFIG) {
                            PrintMessage("Configuration updated. Building cell tower data.");
                            UPDATED_CONFIG = false;
                        }

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
                        List<(int, int)> frequencyCount = gca.RunGCA(CELL_TOWER_DATA, rand);

                        if (frequencyCount == null || frequencyCount.Count == 0)
                        {
                            PrintMessage("Error running the graph coloring algorithm. Exiting application.");
                            return;
                        }

                        // Print the frequency count
                        string[] freqTableHeaders = ["Frequency (MHz)", "Towers Assigned"];
                        string freqTblData = "";
                
                        // create data
                        List<(string, string)> freqTblDataList = new List<(string, string)>(); // stores frequency and Ids
                        
                        for (int f=0; f<frequencyCount.Count; f++)
                        {
                            freqTblDataList.Add((frequencyCount[f].Item1.ToString(), ""));
                        }
                        freqTblDataList.Add(("N/A", ""));
                        // add the frequencies to the list
                        for (int i=0; i<CELL_TOWER_DATA.Count; i++)
                        {
                            int Idx = freqTblDataList.Select(x => x.Item1).ToList().IndexOf(CELL_TOWER_DATA[i].frequency.ToString());
                            if (Idx == -1)
                            {
                                Idx = freqTblDataList.Count - 1; // add to the last item in the list
                            }
                            freqTblDataList[Idx] = (freqTblDataList[Idx].Item1, freqTblDataList[Idx].Item2 + CELL_TOWER_DATA[i].Id + ",");
                        }                        
                        // trim the last comma from the Ids
                        for (int i=0; i<freqTblDataList.Count; i++)
                        {
                            freqTblDataList[i] = (freqTblDataList[i].Item1, freqTblDataList[i].Item2.TrimEnd(','));
                        }

                        // set Second column width to the max length of the Ids
                        int maxLength = freqTblDataList.Select(x => x.Item2.Length).Max() + 4; // add 2 for padding
                        
                        // build the header based on the max length of the Ids
                        (string, string, int, int) tupleHeader2 = BuildTableHeader(freqTableHeaders, maxColWidth:maxLength);
                        string freqTblHeader = tupleHeader2.Item1;
                        string borderTopBottom = tupleHeader2.Item2;
                        int colWidth = tupleHeader2.Item4;

                        for (int i = 0; i < freqTblDataList.Count; i++)
                        {
                            string row = $"|{new string(' ', PADDING/2)}|{new string(' ', 2)}{freqTblDataList[i].Item1}{new string(' ', colWidth - (2 + freqTblDataList[i].Item1.ToString().Length))}|"+
                                $"{new string(' ', 2)}{freqTblDataList[i].Item2}{new string(' ', (maxLength > colWidth ? maxLength : colWidth) - (2 + freqTblDataList[i].Item2.ToString().Length))}|";
                            freqTblData += row + new string(' ', GUI_WIDTH - row.Length + 1) + "|\n";
                        }
                        freqTblData += $"|{new string(' ', PADDING/2)}{borderTopBottom}{new string(' ', GUI_WIDTH - (PADDING / 2 + borderTopBottom.Length))}|";

                        string msg = $"Successfully ran application. Frequencies assigned to cell towers:";
                        Console.WriteLine($"|{new string(' ', PADDING /2)}{msg}{new string (' ', GUI_WIDTH - (PADDING / 2 + msg.Length))}|");
                        Console.WriteLine(freqTblHeader + freqTblData);
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