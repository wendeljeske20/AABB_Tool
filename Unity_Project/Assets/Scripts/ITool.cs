
namespace VIZLab
{
	/// <summary>
	/// Interface that defines the default Tool object. 
	/// </summary>
	public interface ITool
	{
		/// <summary>
		/// Creates a new instance of this Tool's Data.
		/// </summary>
		void CreateData();

		/// <summary>
		/// Interface Getter for this Tool's <see cref="IData"/> object.
		/// </summary>
		/// <returns>This Tool's Data instance.</returns>
		IData GetData();
	}
}
