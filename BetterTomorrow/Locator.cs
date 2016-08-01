using System;
using Android.Locations;
using Android.OS;
using Android.Runtime;

namespace BetterTomorrow
{
	public class Locator : Java.Lang.Object, ILocationListener
	{
		private readonly LocationManager locationManager;
		private Action<Location> onLocationReceived = null;
		public Locator(LocationManager locationManager)
		{
			this.locationManager = locationManager;
		}

		public void RequestLocation(Action<Location> onLocationReceived, Location defaultLocation = null)
		{
		    if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
		    {
		        onLocationReceived?.Invoke(defaultLocation);
		        return;
		    }

			this.onLocationReceived = onLocationReceived;
			var provider = locationManager.GetBestProvider(
				new Criteria { Accuracy = Accuracy.NoRequirement },
				true);

			locationManager.RequestLocationUpdates(provider, 0, 0, this);
		}

		public void OnLocationChanged(Location location)
		{
			onLocationReceived?.Invoke(location);
			onLocationReceived = null;
			locationManager.RemoveUpdates(this);
		}

		public void OnProviderDisabled(string provider) { }
		public void OnProviderEnabled(string provider) { }
		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
	}
}