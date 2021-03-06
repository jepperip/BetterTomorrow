using System.Net;
using System;
using System.IO;

namespace BetterTomorrow.Network
{
	public static class HttpRequestService
	{
		public static bool TryGet(string url, string rest, HttpContentType contentType, out string result)
		{
			result = string.Empty;

			var urlRest = $"{url}{rest}";
			var request = WebRequest.Create(urlRest);
			var type = GetContentType(contentType);
			if(string.IsNullOrEmpty(type))
			{
				Console.WriteLine("Error then getting web request: unknown content type");
				return false;
			}
			request.ContentType = type;
			
			request.Method = "GET";

			Console.WriteLine($"Created web request: {urlRest}");

			try
			{
				using (var response = request.GetResponse() as HttpWebResponse)
				{
					if (response == null)
					{
						return false;
					}

					if(response.StatusCode != HttpStatusCode.OK)
					{
						Console.WriteLine($"Error in GET request: {urlRest} request returned {response.StatusCode}");
						return false;
					}

					using (var reader = new StreamReader(response.GetResponseStream()))
					{
						var content = reader.ReadToEnd();
						if(string.IsNullOrWhiteSpace(content))
						{
							Console.WriteLine($"Error in GET request: response to {urlRest} was empty");
							return false;
						}

						result = content;
					}
				}
			}
			catch (WebException e)
			{
				Console.WriteLine($"Error in GET request: {urlRest}, {e.Message}");
				return false;
			}

			Console.WriteLine($"Http GET request to {urlRest} succeded\nResponse:\n{result}");
			return true;
		}

		private static string GetContentType(HttpContentType contentType)
		{
			switch (contentType)
			{
				case HttpContentType.Json:
					return "application/json";
				case HttpContentType.Xml:
					return "application/xml";
				default:
					return string.Empty;
			}
		}
	}
}