using Android.App;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Widget;
using BetterTomorrow.Network;

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

			//var latView = FindViewById<TextView>(Resource.Id)
			var location = locator.RequestLocation((loc) =>
			{
				var latView = FindViewById<TextView>(Resource.Id.latitudeTextView);
				var longView = FindViewById<TextView>(Resource.Id.longitudeTextView);

				latView.Text = "Lat : " + loc.Latitude;
				longView.Text = "Long : " + loc.Longitude;

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

