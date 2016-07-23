using System;

namespace BetterTomorrow.WeatherDataProvider
{
	public abstract class Weather
	{
		protected Weather(float value)
		{
			Value = value;
		}

		public float Value { get; }

		public abstract bool BetterThan(Weather other);
	}
}