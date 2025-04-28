# Cell Tower Frequency Distribution Console Application

## About the project
In this project I was tasked to implement a console application that would efficiently distribute frequencies to a list of cell towers based on a few constraints. The goal was to ensure that no cell tower is assigned the same frequency as another tower nearby. I found this problem to be very similar to the Graph Colouring Algorithm (but with a twist :)). 

The twist here was that instead of trying to use the least amount of colours (in this case frequencies), we had to try and assign the same frequency to a tower and the tower farthest from it. We had to implement this all while ensuring no two adjacent towers (in this case towers that are “close” to one another) do not share the same frequency.

*Note: The specification document did not define a distance for what is regarded as “close”. Therefore, upon investigation and doing a bit of research, a distance of approximately 500 meters was selected to be regarded as “close”.*

## Prerequisites
- .NET installed on your local environment with C# set up to run a simple "Hello World" program.
  
## How to run the application:
1. Clone the repository locally by running the command
   ```   
     git clone https://github.com/JsteReubsSoftware/CellTowerFrequencies.git
   ```
2. Navigate into the cloned repository:
   ```
     cd CellTowerFrequencies
   ```
3. Build the project by running the command:
   ```
     dotnet build
   ```
4. Run the application using the command:
   ```
     dotnet run
   ```
5. If successful up to this point you should see the following output:
   <div align='center'>
     <img src='https://github.com/JsteReubsSoftware/CellTowerFrequencies/blob/main/data/imgs/welcomeMenu.png' width='80%' height='80%'/>
   </div>
6. Now enter '2' and hit Enter to run the application:
   <div align='center'>
     <img src='https://github.com/JsteReubsSoftware/CellTowerFrequencies/blob/main/data/imgs/runApp.png' width='80%' height='80%'/>
   </div>

## How to test new data:
If you wish to use your custom data other than the current data file, simply following the following steps:

**A. Ensure you have added your custom text file to the `/data/` directory where the other original text file is.**

1. Run the application again but this time select the option
  `Update Configuration by entering '1' and hitting Enter.`
2. Now select option 1 **File Path** if you want to update the data path used by the application.
3. Then enter your new file path as follows (replace *custom_file.txt* with your file name:
   <div align='center'>
     <img src='https://github.com/JsteReubsSoftware/CellTowerFrequencies/blob/main/data/imgs/updateFile.png' width='80%' height='80%'/>
   </div>
4. You can now run the application using your custom data.

*Note: to update the file path either update it using the console application or the config value at line number 443 in the Main.cs file. The same goes for updating the threshold distance value.* 
