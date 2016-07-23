namespace BetterTomorrow.WeatherDataProvider
{
	public class AirTemperature : Weather
	{
		public AirTemperature(float value) :  base(value) { }

		public override bool BetterThan(Weather other) 
		{
			return Value > other.Value;
		}
	}
}