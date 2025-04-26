public class GraphColoringAlgorithm
{
    /***
        * This class implements a graph coloring algorithm to assign frequencies to cell towers.
        * We add a slight adjustment to the traditional algorithm to try and ensure that towers farthest
        * have the same frequency.
        * We also try to ensure that the frequencies used in the cell tower network are balanced.
    ***/
    public int numFrequencies { get; set; } // number of frequencies that can be used
    public List<(int, int)> frequencyCount { get; set; } // number of times each frequency is used

    public GraphColoringAlgorithm(int[] freqRange)
    {
        numFrequencies = freqRange.Length; // number of frequencies that can be used
        frequencyCount = new List<(int, int)>(); // number of times each frequency is used
        for (int i = 0; i < numFrequencies; i++)
        {
            frequencyCount.Add((freqRange[i], 0)); // frequency, count
        }
    }

    public void RunGCA(List<CellTower> cellTowers)
    {
        
    }


}