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

        double distanceLat = (lat - Latitude) * Math.PI / 180;
        double distanceLon = (lon - Longitude) * Math.PI / 180;

        // use Haversine formula
        double a = Math.Pow(Math.Sin(distanceLat / 2), 2) +
                    Math.Cos(Latitude) * Math.Cos(lat) *
                    Math.Pow(Math.Sin(distanceLon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double radius = 6371; // radius of the Earth in kilometers
        double distance = Math.Round(radius * c * 1000, 4); // distance in meters

        // check if the distance is less than the threshold
        if (distance < threshold)
        {
            _nearbyCellTowers.Add((cellTower, distance));
            cellTower.NearbyCellTowers.Add((this, distance)); // add this cell tower to the nearby list of the other cell tower
        }
        else
        {
            _outOfRangeCellTowers.Add((cellTower, distance));
            cellTower.OutOfRangeCellTowers.Add((this, distance)); // add this cell tower to the out of range list of the other cell tower
        }

    }
}