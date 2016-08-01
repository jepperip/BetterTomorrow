using Android.App;
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
using System.Globalization;
using System.Linq;
using BetterTomorrow.UI.Views;
using BetterTomorrow.WheaterData;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private Locator locator;
		private AnimationStack animationStack;
		private View mainView;
	    private static readonly DateTime Today = DateTime.Now;
	    private DateTime sampleTime = Today.AddDays(1);

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

            var weatherSymbolView = FindViewById<ImageView>(Resource.Id.weatherSymbol);
            weatherSymbolView.SetImageResource(Resource.Drawable.s_1);

            SetSampleTime();

            if (IsConnected)
		    {
                locator.RequestLocation(OnLocationReceived);
		    }
		}

		private void OnLocationReceived(Location loc)
		{
			var latView = FindViewById<TextView>(Resource.Id.latitudeTextView);
			var longView = FindViewById<TextView>(Resource.Id.longitudeTextView);

			latView.Text = "Lat : " + loc.Latitude;
			longView.Text = "Long : " + loc.Longitude;

			SmhiResponse response;
			if(!TryGetResponse(loc, out response))
			{
				return;
			}

			var now = DateTime.Now;

			float currentDayAverage = 0.0f;
			int currentDayCount = 0;
			float nextDayAverage = 0.0f;
			int nextDayCount = 0;

			foreach (var timeSerie in response.TimeSeries)
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

			var tomorrowsTimeSerie = response
				.TimeSeries
				.First(
					s => s.ValidTime.DayOfYear == sampleTime.DayOfYear && s.ValidTime.Hour == sampleTime.Hour);

		    var items = WeatherFactory.GetAll(tomorrowsTimeSerie).ToList();

		    var list = FindViewById<ListView>(Resource.Id.MyListView);
            list.Adapter = new WeatherElementListAdapter(this, items);

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

		    SetWeatherSymbol(WeatherFactory.CreateWeatherSymbol(tomorrowsTimeSerie));
		}

	    private void SetSampleTime()
	    {
	        var timeView = FindViewById<TextView>(Resource.Id.date);
	        timeView.Text = sampleTime.ToString(CultureInfo.InvariantCulture);
	    }

	    private void SetWeatherSymbol(WeatherElementModel weatherSymbolData)
	    {
            var weatherSymbolView = FindViewById<ImageView>(Resource.Id.weatherSymbol);

	        int weatherNr = (int)weatherSymbolData.Value;
	        int resId = Resources.GetIdentifier($"s_{weatherNr}", "drawable", PackageName);
            weatherSymbolView.SetImageResource(resId);
        }

	    private bool TryGetResponse(Location loc, out SmhiResponse response)
		{
			response = null;
			var longitude = (int)loc.Longitude;
			var latitude = (int)loc.Latitude;

			string formattedRest = "/api/category/pmp2g/version/2/geotype/point/" +
				$"lon/{longitude}/lat/{latitude}/data.json";

			string jsonData;

			if (!HttpRequestService.TryGet(
				GetString(Resource.String.SMHI_SERVICE_URL),
				formattedRest,
				HttpContentType.Json, out jsonData))
			{
				return false;
			}

			return new SmhiJsonParser().TryParse(jsonData, out response);
		}

		private void Animate()
		{
			var timeView = FindViewById<TextView>(Resource.Id.date);
			var latTextView = FindViewById<TextView>(Resource.Id.latitudeTextView);
			var longTextView = FindViewById<TextView>(Resource.Id.longitudeTextView);
            var weatherSymbolView = FindViewById<ImageView>(Resource.Id.weatherSymbol);
            var duration = 800;

            animationStack.PushAnimation(new ViewPositionAnimator(
                weatherSymbolView,
                0.0f,
                weatherSymbolView.GetX(),
                weatherSymbolView.GetY(),
                weatherSymbolView.GetY(),
                duration));

            animationStack.AddDelay(200);

            animationStack.PushAnimation(new ViewPositionAnimator(
                timeView,
				0.0f,
                timeView.GetX(),
                timeView.GetY(),
                timeView.GetY(),
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

