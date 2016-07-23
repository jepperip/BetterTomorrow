using Newtonsoft.Json;
using BetterTomorrow.Network.SMHI.Data;
using System;

namespace BetterTomorrow.Network.SMHI
{
	public class SmhiJsonParser : IJsonParser<SmhiResponse>
	{
		public bool TryParse(string content, out SmhiResponse result)
		{
			result = new SmhiResponse();
			try
			{
				result = JsonConvert.DeserializeObject<SmhiResponse>(content);
			}
			catch (JsonException e)
			{
				Console.WriteLine($"Error when parsing JSON: {e.Message}");
				return false;
			}

			return true;
		}
	}
}