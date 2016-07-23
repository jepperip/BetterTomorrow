namespace BetterTomorrow.Network
{
	public interface IJsonParser<T>
	{
		bool TryParse(string content, out T result);
	}
}