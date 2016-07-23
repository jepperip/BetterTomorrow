using System;

namespace BetterTomorrow.WheaterDataProvider
{
	public abstract class Wheater
	{
		protected Wheater(float value)
		{
			Value = value;
		}

		public float Value { get; }

		public abstract bool BetterThan(Wheater other);
	}
}