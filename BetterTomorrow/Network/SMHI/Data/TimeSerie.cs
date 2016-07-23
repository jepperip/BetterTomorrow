
using System;
using System.Collections.Generic;

namespace BetterTomorrow.Network.SMHI.Data
{
	public class TimeSerie
	{
		public DateTime ValidTime { get; set; }
		public IList<Parameter> Parameters { get; set; }
	}
}