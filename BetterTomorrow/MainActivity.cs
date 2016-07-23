using Android.App;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Widget;
using BetterTomorrow.Network;
using BetterTomorrow.Network.SMHI;
using System;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		Locator locator;
		
		protected override void OnCreate(Bundle bundle)
		{
			var test = MultiPointGetRequestBuilder.Build(DateTime.Now, WheaterProperty.AirTemperature);
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

			//var latView = FindViewById<TextView>(Resource.Id)
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

