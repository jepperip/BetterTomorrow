using Android.App;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Widget;
using BetterTomorrow.Network;
using BetterTomorrow.Network.SMHI;
using BetterTomorrow.Network.SMHI.Data;
using System;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		Locator locator;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			locator = new Locator((LocationManager)GetSystemService(LocationService));

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
		}

		protected override void OnResume()
		{
			base.OnResume();
			var statusView = FindViewById<CheckBox>(Resource.Id.OnlineCheckBox);
			statusView.Checked = IsConnected;

			var location = locator.RequestLocation((loc) =>
			{
				var latView = FindViewById<TextView>(Resource.Id.latitudeTextView);
				var longView = FindViewById<TextView>(Resource.Id.longitudeTextView);

				latView.Text = "Lat : " + loc.Latitude;
				longView.Text = "Long : " + loc.Longitude;

				string jsonData;
				var res = HttpRequestService.TryGet(
					GetString(Resource.String.SMHI_SERVICE_URL),
					"/api/category/pmp2g/version/2/geotype/point/" +
					$"lon/{loc.Longitude.ToString("F6")}/lat/{loc.Latitude.ToString("F6")}/data.json",
					HttpContentType.Json, out jsonData);

				SmhiResponse r;
				if(new SmhiJsonParser().TryParse(jsonData, out r))
				{
					var now = DateTime.Now;

					float currentDayAverage = 0.0f;
					int currentDayCount = 0;
					float nextDayAverage = 0.0f;
					int nextDayCount = 0;
					foreach(var timeSerie in r.TimeSeries)
					{
						if(timeSerie.ValidTime.DayOfYear > now.DayOfYear + 2)
						{
							break;
						}

						foreach (var parameter in timeSerie.Parameters)
							{
								if(string.Equals(parameter.Name,"t"))
								{
									if(timeSerie.ValidTime.DayOfYear == now.DayOfYear)
									{
										currentDayAverage += parameter.Values[0];
										currentDayCount++;
									}
									else
									{
										nextDayAverage += parameter.Values[0];
										nextDayCount++;
									}

									break;
								}
							}
					}

					currentDayAverage /= currentDayCount;
					nextDayAverage /= nextDayCount;
					var delta = nextDayAverage - currentDayAverage;
					var resultView = FindViewById<TextView>(Resource.Id.resultTextView);
					resultView.Text = nextDayAverage > currentDayAverage ?
					$"Hitler was right all along {delta}" :
					$"The holocaust never happened {delta}";
				}
			});
		}

		private bool IsConnected
		{
			get
			{
				ConnectivityManager connectManager = (ConnectivityManager)GetSystemService(ConnectivityService);
				NetworkInfo activeConnection = connectManager.ActiveNetworkInfo;
				return activeConnection != null && activeConnection.IsConnected;
			}
		}
	}
}

