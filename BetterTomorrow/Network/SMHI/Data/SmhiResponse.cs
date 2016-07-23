using System;
using System.Collections.Generic;

namespace BetterTomorrow.Network.SMHI.Data
{
	public class SmhiResponse
	{
		public DateTime ApprovedTime { get; set; }
		public DateTime ReferenceTime { get; set; }
		public object Geometry { get; set; }
		public IList<TimeSerie> TimeSeries { get; set; }
	}
}