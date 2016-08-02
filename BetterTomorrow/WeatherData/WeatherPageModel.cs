using System;
using System.Collections.Generic;

namespace BetterTomorrow.WeatherData
{
    public class WeatherPageModel
    {
        public WeatherPageModel(
            IReadOnlyList<WeatherElementModel> elements,
            int weatherSymbol,
            DateTime sampleTime,
            Location location,
           bool wasHitlerRight)
        {
            Elements = elements;
            WeatherSymbol = weatherSymbol;
            SampleTime = sampleTime;
            Location = location;
            WasHitlerRight = wasHitlerRight;
        }

        public IReadOnlyList<WeatherElementModel> Elements { get; }
        public int WeatherSymbol { get; }
        public DateTime SampleTime { get; }
        public Location Location { get; }
        public bool WasHitlerRight { get; }
    }
}