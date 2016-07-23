namespace BetterTomorrow.WheaterDataProvider
{
	public class AirTemperature : Wheater
	{
		public AirTemperature(float value) :  base(value) { }

		public override bool BetterThan(Wheater other) 
		{
			return Value > other.Value;
		}
	}
}