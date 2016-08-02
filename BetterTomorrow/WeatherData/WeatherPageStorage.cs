using System.Collections.Generic;

namespace BetterTomorrow.WeatherData
{
    public static class WeatherPageStorage
    {
        public static readonly IDictionary<int, WeatherPageModel> Storage = new Dictionary<int, WeatherPageModel>();
    }
}