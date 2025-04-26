public class CellTower
{
    public string Id { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Northing { get; set; }
    public string Easting { get; set; } 

    private List<CellTower> _nearbyCellTowers; // stores the nearby cell towers based on "close threshold"

    public List<CellTower> NearbyCellTowers
    {
        get { return _nearbyCellTowers; }
        set { _nearbyCellTowers = value; }
    }

    public CellTower(string id, string latitude, string longitude, string northing, string easting)
    {
        Id = id;
        Latitude = latitude;
        Longitude = longitude;
        Northing = northing;
        Easting = easting;
        _nearbyCellTowers = [];
    }

    // This method will act as adding an "edge" to the graph of cell towers
    public void LinkCellTower(CellTower cellTower)
    {
        _nearbyCellTowers.Add(cellTower);
    }


    

}