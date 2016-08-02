using System.Globalization;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using BetterTomorrow.WeatherData;

namespace BetterTomorrow.UI.Views
{
    public class WeatherPageFragment : Fragment
    {
        private WeatherPageModel model;

        public static WeatherPageFragment CreateInstance(WeatherPageModel model)
        {
            var result = new WeatherPageFragment();
            int id = model.GetHashCode();
            WeatherPageStorage.Storage[id] = model;

            var args = new Bundle();
            args.PutInt("id", id);
            result.Arguments = args;
            return result;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            int id = Arguments?.GetInt("id") ?? 0;
            model = WeatherPageStorage.Storage[id];
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            int weatherSymbol = model.WeatherSymbol;
            var location = model.Location;
            var date = model.SampleTime;
            var hitler = model.WasHitlerRight;
            var weatherData = model.Elements;
            
            var view = inflater.Inflate(Resource.Layout.WeatherPageView, container, false);
            
            view.FindViewById<TextView>(Resource.Id.weatherPage_date).Text =
                date.ToString(CultureInfo.InvariantCulture);
            view.FindViewById<TextView>(Resource.Id.weatherPage_latitudeTextView).Text =
                $"Lat:{location.Latitude}";
            view.FindViewById<TextView>(Resource.Id.weatherPage_longitudeTextView).Text =
                $"Lon:{location.Longitude}";
            view.FindViewById<TextView>(Resource.Id.weatherPage_resultTextView).Text =
                hitler ? "Hitler was right all along" : "The holocaust never happened";
            view.FindViewById<ImageView>(Resource.Id.weatherPage_weatherSymbol)
                .SetImageResource(GetWeatherSymbolId(weatherSymbol, container.Context.PackageName));
            view.FindViewById<ListView>(Resource.Id.weatherPage_elementList).Adapter =
                new WeatherElementListAdapter(container.Context, weatherData);

            return view;
        }

        private int GetWeatherSymbolId(int weatherSymbol, string packageName)
        {
            return Resources.GetIdentifier($"s_{weatherSymbol}", "drawable", packageName);
        }
    }
}