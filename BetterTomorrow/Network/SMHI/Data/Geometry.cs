using System.Collections.Generic;

namespace BetterTomorrow.Network.SMHI.Data
{
	class Geometry
	{
		public string Type { get; set; }
		IList<double> Coordinates { get; set; }
	}
}