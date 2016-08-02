using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using BetterTomorrow.WeatherData;

namespace BetterTomorrow.UI.Views
{
    public class WeatherElementListAdapter : BaseAdapter<WeatherElementModel>
    {
        private readonly IReadOnlyList<WeatherElementModel> items;
        private readonly Context context;

        public WeatherElementListAdapter(Context context, IReadOnlyList<WeatherElementModel> items)
        {
            this.items = items;
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = convertView ??
                LayoutInflater.From(context).Inflate(Resource.Layout.WeatherElementView, null, false);

            row.FindViewById<TextView>(Resource.Id.WeatherElement_Name).Text =
                items[position].Name;
            row.FindViewById<TextView>(Resource.Id.WeatherElement_Value).Text =
                items[position].Value.ToString(CultureInfo.InvariantCulture);
            row.FindViewById<TextView>(Resource.Id.WeatherElement_Unit).Text =
                items[position].Unit.ToString(CultureInfo.InvariantCulture);

            return row;
        }

        public override int Count => items.Count;

        public override WeatherElementModel this[int position] => items[position];
    }
}