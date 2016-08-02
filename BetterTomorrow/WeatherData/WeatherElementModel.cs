namespace BetterTomorrow.WeatherData
{
	public class WeatherElementModel
	{
		public WeatherElementModel(string name, float value, string unit)
		{
			Name = name;
		    Value = value;
			Unit = unit;
		}

		public string Name { get; }
		public float Value { get; }
		public string Unit { get; }
	}
}