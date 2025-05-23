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


    public void IncrementFrequencyCount(CellTower currTower)
    {
        for (int i = 0; i < numFrequencies; i++)
        {
            if (frequencyCount[i].Item1 == currTower.frequency)
            {
                frequencyCount[i] = (frequencyCount[i].Item1, frequencyCount[i].Item2 + 1); // increment the count of the frequency used
                break;
            }
        }
    }

    public List<(int, int)> RunGCA(List<CellTower> cellTowers, Random random)
    {
        try
        {
            // check if the list contains towers
            if (cellTowers.Count == 0)
            {
                throw new Exception("No cell towers to assign frequencies to.");
            }

            // check if the list contains frequencies
            if (numFrequencies == 0)
            {
                throw new Exception("No frequencies available to assign to cell towers.");
            }

            // if there is just one tower, assign any frequency to it
            if (cellTowers.Count == 1)
            {
                int randomFreqIdx = random.Next(0, numFrequencies);
                cellTowers[0].frequency = frequencyCount[randomFreqIdx].Item1; // assign the frequency to the tower
                frequencyCount[randomFreqIdx] = (frequencyCount[randomFreqIdx].Item1, frequencyCount[randomFreqIdx].Item2 + 1); // increment the count of the frequency used
                return frequencyCount;
            }
            
            // If there are more than one tower, we run the graph coloring algorithm
            for (int ctIdx=0; ctIdx<cellTowers.Count; ctIdx++)
            {
                CellTower currTower = cellTowers[ctIdx];

                // if current tower has a frequency assigned, we skip it
                if (currTower.frequency != -1) // frequency assigned
                {
                    continue; // go to the next tower
                }

                // first we retrieve the neighbouring and non-neighbouring towers
                List<(CellTower, double)> nearbyTowers = currTower.NearbyCellTowers;
                List<(CellTower, double)> outOfRangeTowers = currTower.OutOfRangeCellTowers;

                if (outOfRangeTowers.Count > 0) // there are some towers out of range
                {
                    // find farthest tower
                    List<double> distance = outOfRangeTowers.Select(t => t.Item2).ToList();
                    double maxDistance = distance.Max(); // find the maximum distance
                    int maxDistIdx = distance.IndexOf(maxDistance); // find the index of the maximum distance
                    CellTower farthestTower = outOfRangeTowers[maxDistIdx].Item1; // find the farthest tower

                    // if current tower is the first i.e. no frequency assigned to any tower, 
                    // we assign any frequency to both the current tower and farthest tower
                    if (ctIdx == 0) {
                        int randomFreqIdx = random.Next(0, numFrequencies);
                        currTower.frequency = frequencyCount[randomFreqIdx].Item1;
                        farthestTower.frequency = currTower.frequency;
                        frequencyCount[randomFreqIdx] = (frequencyCount[randomFreqIdx].Item1, frequencyCount[randomFreqIdx].Item2 + 1);
                        continue; // go to the next tower
                    }
                    
                    // Current Tower is not the first tower
                    // check if current tower has no neighbours
                    if (nearbyTowers.Count == 0)
                    {
                        //the tower has no neighbours so it can be assigned the same frequency as the farthest tower if the farthest tower has a frequency
                        if (farthestTower.frequency == -1) // farthest tower has no frequency assigned
                        {
                            // assign any frequency to the current tower
                            int randomFreqIdx = random.Next(0, numFrequencies);
                            currTower.frequency = frequencyCount[randomFreqIdx].Item1;
                            farthestTower.frequency = currTower.frequency; // assign the same frequency to the farthest tower
                            // increase the frequency count twice since we assigned the same frequency to both towers
                            IncrementFrequencyCount(currTower);
                            IncrementFrequencyCount(farthestTower);
                        }
                        else // the farthest tower has a frequency assigned
                        {
                            // assign the same frequency as the farthest tower
                            currTower.frequency = farthestTower.frequency;
                            // increase the frequency count
                            IncrementFrequencyCount(currTower);
                        }
                    }
                    else // the tower has neighbours so we first need to check if we can assign the same frequency as the farthest tower
                    {
                        // retrieve available frequencies
                        List<int> freqRange = frequencyCount.Select(fc => fc.Item1).ToList();

                        foreach ((CellTower, double) t in nearbyTowers)
                        {
                            if (freqRange.Contains(t.Item1.frequency)) // if the frequency is in the range of frequencies available
                            {
                                freqRange.Remove(t.Item1.frequency); // remove the frequency from the range
                            }
                        }

                        if (freqRange.Count > 0) // there are available frequencies
                        {
                            if (farthestTower.frequency != -1 && freqRange.Contains(farthestTower.frequency))
                            {
                                currTower.frequency = farthestTower.frequency; // assign the same frequency to the current tower                                
                                // increase the frequency count
                                IncrementFrequencyCount(currTower);
                            }
                            else if (farthestTower.frequency != -1 && !freqRange.Contains(farthestTower.frequency))// assign any frequency from the available frequencies
                            {
                                int randomFreqIdx = random.Next(0, freqRange.Count);
                                currTower.frequency = freqRange[randomFreqIdx];
                                
                                // increase the frequency count 
                                IncrementFrequencyCount(currTower);
                            }
                            else // assign any frequency from the available frequencies
                            {
                                int randomFreqIdx = random.Next(0, freqRange.Count);
                                currTower.frequency = freqRange[randomFreqIdx];
                                farthestTower.frequency = currTower.frequency; // assign the same frequency to the farthest tower
                                
                                // increase the frequency count twice since we assigned the same frequency to both towers
                                IncrementFrequencyCount(currTower);
                                IncrementFrequencyCount(farthestTower);
                            }
                        }
                    }
                }
                // else
                // {
                //     // the tower has no out of range towers so we can assign any frequency to it
                // }
            }

            // return the frequency count
            return frequencyCount;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in GraphColoringAlgorithm: " + ex.Message);
        }
    }


}