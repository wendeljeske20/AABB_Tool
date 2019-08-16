namespace VIZLab
{
	/// <summary>
	/// Interface that defines the default Data object.
	/// </summary>
	public interface IData
	{
		/// <summary>
		/// Performs Encoding of the Data object to a string.
		/// </summary>
		/// <returns>A string containing the serialized Data object.</returns>
		string Encode();

		/// <summary>
		/// Performs Decoding of the Data object from a string.
		/// Overrides this Data internal state.
		/// </summary>
		void Decode(string objectData);
	}
}