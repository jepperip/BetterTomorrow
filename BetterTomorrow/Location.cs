namespace BetterTomorrow
{
    public class Location
    {
        public Location(float longitude = 0, float latitude = 0)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}