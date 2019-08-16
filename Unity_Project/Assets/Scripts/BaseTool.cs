using UnityEngine;

namespace VIZLab
{
	/// <summary>
	/// Definition of base Tool class with a given Data.
	/// </summary>
	/// <typeparam name="TData">The Data type used by this Tool.</typeparam>
	public class BaseTool<TData> : MonoBehaviour, ITool where TData : IData, new()
	{
		/// <summary>
		/// This Tool's Data instance.
		/// </summary>
		protected TData data = new TData();

		/// <summary>
		/// Creates a new instance of this Tool's Data.
		/// </summary>
		public void CreateData()
		{
			data = new TData();
		}

		/// <summary>
		/// Interface Getter for this Tool's <see cref="IData"/> object.
		/// </summary>
		/// <returns>This Tool's Data instance.</returns>
		public IData GetData()
		{
			return data;
		}
	}
}

