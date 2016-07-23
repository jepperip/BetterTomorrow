using System.Collections.Generic;

namespace BetterTomorrow.Network.SMHI.Data
{
	public class Parameter
	{
		public string Name { get; set; }
		public string LevelType { get; set; }
		public int Level { get; set; }
		public IList<float> Values { get; set; }
	}
}