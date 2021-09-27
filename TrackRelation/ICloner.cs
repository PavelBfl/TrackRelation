namespace Track.Relation
{
	public interface ICloner<T>
	{
		T Clone(T value);
	}
}
