using Android.App;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Views;
using BetterTomorrow.Network;
using BetterTomorrow.Network.SMHI;
using BetterTomorrow.Network.SMHI.Data;
using BetterTomorrow.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.View;
using Android.Widget;
using BetterTomorrow.UI.Views;
using BetterTomorrow.WeatherData;

namespace BetterTomorrow
{
	[Activity(Label = "BetterTomorrow", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
	public class MainActivity : Activity, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private Locator locator;
		private AnimationStack animationStack;
		private View mainView;
	    private static readonly DateTime Today = DateTime.Now;
        private static readonly Location DefaultLocation = new Location(13, 55);

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            locator = new Locator((LocationManager)GetSystemService(LocationService));
            animationStack = new AnimationStack();

            // Set our view from the "main" layout resource
            mainView = FindViewById<View>(Resource.Id.mainView);
            mainView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

		protected override void OnResume()
		{
			base.OnResume();

            if (IsConnected)
            {
                locator.RequestLocation(OnLocationReceived, DefaultLocation);
            }
        }

		private void OnLocationReceived(Location loc)
        {
            SmhiResponse response;
            if (!TryGetResponse(loc, out response))
            {
                return;
            }

		    DisableProgressView();

            bool hitler = WasHitleRight(response, Today);

            var test = new List<WeatherPageModel>();
		    foreach (var timeSerie in response.TimeSeries)
		    {
		        //var elements = WeatherFactory.GetAll(timeSerie);
		        var elements = new[]
		        {
		            WeatherFactory.CreateTempature(timeSerie),
                    WeatherFactory.CreateThunderProbability(timeSerie),
                    WeatherFactory.CreateWindSpeed(timeSerie),
                    WeatherFactory.CreatePrecipitation(timeSerie)
		        };
		        var weatherSymbol = WeatherFactory.CreateWeatherSymbol(timeSerie);
                var page = new WeatherPageModel(elements.ToList(), (int)weatherSymbol.Value, timeSerie.ValidTime, loc, hitler);
                test.Add(page);
		    }

            var adapter = new WeatherPageAdapter(FragmentManager, test);

            var pager = FindViewById<ViewPager>(Resource.Id.mainView_pager);
            pager.Adapter = adapter;

            //animationStack.PushAnimation(new TextViewColorAnimator(
            //    resultView,
            //    0,
            //    resultView.TextColors.DefaultColor,
            //    2000));

            //var loadingSpinner = FindViewById<ProgressBar>(Resource.Id.loadingSpinner);
            //animationStack.PushAnimation(new ViewFadeAnimator(loadingSpinner, 10, 100, 0));

            //var weatherSymbolView = FindViewById<ImageView>(Resource.Id.weatherSymbol);
            //animationStack.PushAnimation(new ViewFadeAnimator(weatherSymbolView, 2000, to: 0.5f));

            //animationStack.Start();
            //animationStack.Clear();

            //SetWeatherSymbol(WeatherFactory.CreateWeatherSymbol(tomorrowsTimeSerie));
        }

	    private void DisableProgressView()
	    {
	        var progressView = FindViewById<ProgressBar>(Resource.Id.mainView_loadingSpinner);
	        progressView.Enabled = false;
            progressView.Visibility = ViewStates.Gone;
	    }

	    private static bool WasHitleRight(SmhiResponse response, DateTime now)
        {
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

            currentDayAverage /= currentDayCount;
            nextDayAverage /= nextDayCount;
            var delta = nextDayAverage - currentDayAverage;

            return nextDayAverage > currentDayAverage;
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
			//var timeView = FindViewById<TextView>(Resource.Id.date);
			//var latTextView = FindViewById<TextView>(Resource.Id.latitudeTextView);
			//var longTextView = FindViewById<TextView>(Resource.Id.longitudeTextView);
		    
   //         var duration = 800;

   //         animationStack.PushAnimation(new ViewPositionAnimator(
   //             timeView,
			//	0.0f,
   //             timeView.GetX(),
   //             timeView.GetY(),
   //             timeView.GetY(),
			//	duration));

			//animationStack.AddDelay(200);

			//animationStack.PushAnimation(new ViewPositionAnimator(
			//	latTextView,
			//	0.0f,
			//	latTextView.GetX(),
			//	latTextView.GetY(),
			//	latTextView.GetY(),
			//	duration));

			//animationStack.PushAnimation(new ViewPositionAnimator(
			//	longTextView,
			//	0.0f,
			//	longTextView.GetX(),
			//	longTextView.GetY(),
			//	longTextView.GetY(),
			//	duration));

   //         animationStack.Start();
			//animationStack.Clear();
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

