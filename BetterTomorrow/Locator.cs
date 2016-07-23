using System;
using Android.Locations;
using Android.OS;
using Android.Runtime;

namespace BetterTomorrow
{
	class Locator : Java.Lang.Object, ILocationListener
	{
		private LocationManager locationManager;
		private Location currentLocation = null;
		private Action<Location> onLocationReceived = null;
		public Locator(LocationManager locationManager)
		{
			this.locationManager = locationManager;
		}

		public Location RequestLocation(Action<Location> onLocationReceived)
		{
			this.onLocationReceived = onLocationReceived;
			var provider = locationManager.GetBestProvider(
				new Criteria { Accuracy = Accuracy.Coarse },
				true);

			locationManager.RequestLocationUpdates(provider, 0, 0, this);

			return currentLocation;
		}

		public void OnLocationChanged(Location location)
		{
			onLocationReceived(location);
			locationManager.RemoveUpdates(this);
		}

		public void OnProviderDisabled(string provider) { }
		public void OnProviderEnabled(string provider) { }
		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
	}
}