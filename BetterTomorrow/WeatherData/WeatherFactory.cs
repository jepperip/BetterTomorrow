using System.Collections.Generic;
using System.Linq;
using BetterTomorrow.Network.SMHI.Data;

namespace BetterTomorrow.WeatherData
{
	public static class WeatherFactory
	{
		private static class ParameterNames
		{
			// Prec = precipitation = nederbörd.
			public const string Temp = "t";
			public const string AirPressure = "msi";
			public const string HorizontalVisibility = "vis";
			public const string WindDirection = "wd";
			public const string WindSpeed = "ws";
			public const string RelativeHumidity = "r";
			public const string ThunderProbability = "tstm";
			public const string WindGustSpeed = "gust";
			public const string MinPrecIntensity = "pmin";
			public const string MaxPrecIntensity = "pmax";
			public const string PrecCategory = "pcat";
			public const string MeanPrecIntensity = "pmean";
			public const string MedianPrecIntensity = "pmedian";
			public const string WeatherSymbol = "Wsymb";
		}

		private static readonly IDictionary<string, KeyValuePair<string, string>> units = new Dictionary<string, KeyValuePair<string, string>>
		{
			{ ParameterNames.Temp, new KeyValuePair<string, string>("C", "Temperature") },
			{ ParameterNames.AirPressure, new KeyValuePair<string, string>("hPa", "Air Pressure") },
			{ ParameterNames.HorizontalVisibility, new KeyValuePair<string, string>("km", "Horizontal Visibility") },
			{ ParameterNames.WindDirection, new KeyValuePair<string, string>("degree", "Wind Direction") },
			{ ParameterNames.WindSpeed, new KeyValuePair<string, string>("m/s", "Wind Speed") },
			{ ParameterNames.RelativeHumidity, new KeyValuePair<string, string>("%", "Relative Humidity") },
			{ ParameterNames.ThunderProbability, new KeyValuePair<string, string>("%", "Thunder Probability") },
			{ ParameterNames.WindGustSpeed, new KeyValuePair<string, string>("m/s", "Wind Gust Speed") },
			{ ParameterNames.MinPrecIntensity, new KeyValuePair<string, string>("mm/h", "Minimum Precipitation Intensity") },
			{ ParameterNames.MaxPrecIntensity, new KeyValuePair<string, string>("mm/h", "Maximum Precipitation Intensity") },
			{ ParameterNames.PrecCategory, new KeyValuePair<string, string>("category", "Precipitation Category") },
			{ ParameterNames.MeanPrecIntensity, new KeyValuePair<string, string>("mm/h", "Mean Precipitation Intensity") },
			{ ParameterNames.MedianPrecIntensity, new KeyValuePair<string, string>("mm/h", "Median Precipitation Intensity") },
			{ ParameterNames.WeatherSymbol, new KeyValuePair<string, string>("code", "Whater Symbol") }
		};

		public static WeatherElementModel CreateTempature(TimeSerie timeSerie)
		{
			return ConvertToWeatherModel(timeSerie, ParameterNames.Temp);
		}

		public static WeatherElementModel CreateAirPressure(TimeSerie timeSerie)
		{
			return ConvertToWeatherModel(timeSerie, ParameterNames.AirPressure);
		}

		public static WeatherElementModel CreateWindDirection(TimeSerie timeSerie)
		{
			return ConvertToWeatherModel(timeSerie, ParameterNames.WindDirection);
		}

		public static WeatherElementModel CreateWindSpeed(TimeSerie timeSerie)
		{
			return ConvertToWeatherModel(timeSerie, ParameterNames.WindSpeed);
		}

		public static WeatherElementModel CreateThunderProbability(TimeSerie timeSerie)
		{
			return ConvertToWeatherModel(timeSerie, ParameterNames.ThunderProbability);
		}

	    public static WeatherElementModel CreateWeatherSymbol(TimeSerie timeSerie)
	    {
            return ConvertToWeatherModel(timeSerie, ParameterNames.WeatherSymbol);
        }

	    public static IEnumerable<WeatherElementModel> GetAll(TimeSerie timeSerie)
	    {
	        return units.Select(s => ConvertToWeatherModel(timeSerie, s.Key)).Where(s => s != null);
	    }

		private static WeatherElementModel ConvertToWeatherModel(TimeSerie timeSerie, string parameterName)
		{
			Parameter parameter;
            if(!TryGetParameter(timeSerie, parameterName, out parameter))
		    {
		        return null;
		    }
		    var parameterInfo = units[parameterName];

			return new WeatherElementModel(parameterInfo.Value,
				parameter.Values.First(),
				parameterInfo.Key);
		}

		private static bool TryGetParameter(TimeSerie timeSerie, string parameterName, out Parameter parameter)
		{
			parameter = timeSerie.Parameters.FirstOrDefault(s => string.Equals(s.Name, parameterName));
		    return parameter != null;
		}
	}
}