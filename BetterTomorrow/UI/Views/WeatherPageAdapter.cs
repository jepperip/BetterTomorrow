using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Support.V13.App;
using BetterTomorrow.WeatherData;

namespace BetterTomorrow.UI.Views
{
    public class WeatherPageAdapter : FragmentStatePagerAdapter
    {
        private readonly IReadOnlyList<WeatherPageModel> pages;

        public WeatherPageAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public WeatherPageAdapter(
            FragmentManager fragmentManager,
            IReadOnlyList<WeatherPageModel> weatherPages) 
            : base(fragmentManager)
        {
            pages = weatherPages;
        }

        public override int Count => pages.Count;

        public override Fragment GetItem(int position) =>
            WeatherPageFragment.CreateInstance(pages[position]);

    }
}