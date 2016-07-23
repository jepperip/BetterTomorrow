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
			var request = HttpWebRequest.Create(urlRest);
			request.ContentType = GetContentType(contentType);
			request.Method = "GET";

			Console.WriteLine($"Created web request: {urlRest}");

			using (var response = request.GetResponse() as HttpWebResponse)
			{
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
					return "";
			}
		}
	}
}