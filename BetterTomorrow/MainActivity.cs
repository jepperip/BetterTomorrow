using Android.Animation;
using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using BetterTomorrow.Network;
using BetterTomorrow.Network.SMHI;
using BetterTomorrow.Network.SMHI.Data;
using BetterTomorrow.UI;
using System;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, ViewTreeObserver.IOnGlobalLayoutListener
	{
		Locator locator;
		AnimationStack animationStack;
		View mainView;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			locator = new Locator((LocationManager)GetSystemService(LocationService));
			animationStack = new AnimationStack();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			mainView = FindViewById<View>(Resource.Id.mainView);
			mainView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
		}

		protected override void OnResume()
		{
			base.OnResume();
			var statusView = FindViewById<CheckBox>(Resource.Id.OnlineCheckBox);
			statusView.Checked = IsConnected;

			locator.RequestLocation(OnLocationReceived);
		}

		private void OnLocationReceived(Location loc)
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
			if (new SmhiJsonParser().TryParse(jsonData, out r))
			{
				var now = DateTime.Now;

				float currentDayAverage = 0.0f;
				int currentDayCount = 0;
				float nextDayAverage = 0.0f;
				int nextDayCount = 0;
				foreach (var timeSerie in r.TimeSeries)
				{
					if (timeSerie.ValidTime.DayOfYear > now.DayOfYear + 2)
					{
						break;
					}

					foreach (var parameter in timeSerie.Parameters)
					{
						if (string.Equals(parameter.Name, "t"))
						{
							if (timeSerie.ValidTime.DayOfYear == now.DayOfYear)
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

				animationStack.PushAnimation(new TextViewColorAnimator(
						resultView,
						0,
						resultView.TextColors.DefaultColor,
						2000));
				animationStack.Start();
				animationStack.Clear();
			}
		}

		private void Animate()
		{
			var statusView = FindViewById<CheckBox>(Resource.Id.OnlineCheckBox);
			var onlineTextView = FindViewById<TextView>(Resource.Id.textView1);
			var latTextView = FindViewById<TextView>(Resource.Id.latitudeTextView);
			var longTextView = FindViewById<TextView>(Resource.Id.longitudeTextView);
			var duration = 800;

			animationStack.PushAnimation(new ViewPositionAnimator(
				onlineTextView,
				0.0f,
				onlineTextView.GetX(),
				onlineTextView.GetY(),
				onlineTextView.GetY(),
				duration));

			animationStack.AddDelay(200);

			animationStack.PushAnimation(new ViewPositionAnimator(
				statusView,
				0.0f,
				statusView.GetX(),
				statusView.GetY(),
				statusView.GetY(),
				duration));

			animationStack.AddDelay(200);

			animationStack.PushAnimation(new ViewPositionAnimator(
				latTextView,
				0.0f,
				latTextView.GetX(),
				latTextView.GetY(),
				latTextView.GetY(),
				duration));

			animationStack.PushAnimation(new ViewPositionAnimator(
				longTextView,
				0.0f,
				longTextView.GetX(),
				longTextView.GetY(),
				longTextView.GetY(),
				duration));

			animationStack.Start();
			animationStack.Clear();
		}

		public void OnGlobalLayout()
		{
			mainView.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
			Animate();
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

