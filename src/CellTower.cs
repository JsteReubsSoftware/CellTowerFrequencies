public class CellTower
{
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Northing { get; set; }
    public double Easting { get; set; } 
    public int frequency { get; set; } // frequency assigned to the cell tower

    private List<(CellTower, double)> _nearbyCellTowers; // stores the nearby cell towers based on "close threshold"
    private List<(CellTower, double)> _outOfRangeCellTowers; // stores the cell towers that are out of range i.e. not close

    public List<(CellTower, double)> NearbyCellTowers
    {
        get { return _nearbyCellTowers; }
        set { _nearbyCellTowers = value; }
    }

    public List<(CellTower, double)> OutOfRangeCellTowers
    {
        get { return _outOfRangeCellTowers; }
        set { _outOfRangeCellTowers = value; }
    }

    public CellTower(string id, double easting, double northing, double longitude, double latitude)
    {
        Id = id;
        Easting = easting;
        Northing = northing;
        Longitude = longitude;
        Latitude = latitude;
        _nearbyCellTowers = [];
        _outOfRangeCellTowers = [];
        frequency = -1; // default frequency
    }   

    public void AddCellTower(CellTower cellTower, double threshold)
    {
        double lat = cellTower.Latitude;
        double lon = cellTower.Longitude;

        // use Haversine formula
        double a = Math.Pow(Math.Sin((lat - Latitude) * Math.PI / 180 / 2), 2) +
            Math.Cos(Latitude * Math.PI / 180) * Math.Cos(lat * Math.PI / 180) *
            Math.Pow(Math.Sin((lon - Longitude) * Math.PI / 180 / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double r = 6371; // Approximate Radius of Earth in kilometers
        double distance = Math.Round(r * c * 1000, 4); // distance in meters

        // check if the distance is less than the threshold
        if (distance < threshold)
        {
            _nearbyCellTowers.Add((cellTower, distance));
        }
        else
        {
            _outOfRangeCellTowers.Add((cellTower, distance));
        }

    }
}