using Android.App;
using Android.Net;
using Android.OS;
using Android.Widget;
using BetterTomorrow.Network;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
		}

		protected override void OnResume()
		{
			base.OnResume();
			var statusView = FindViewById<CheckBox>(Resource.Id.OnlineCheckBox);
			statusView.Checked = IsConnected;
		}

		private bool IsConnected
		{
			get
			{
				ConnectivityManager connectManager = (ConnectivityManager)GetSystemService(Android.Content.Context.ConnectivityService);
				NetworkInfo activeConnection = connectManager.ActiveNetworkInfo;
				return activeConnection != null && activeConnection.IsConnected;
			}
		}
	}
}

