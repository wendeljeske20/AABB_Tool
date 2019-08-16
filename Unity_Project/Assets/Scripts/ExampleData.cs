using VIZLab;
using System.Collections.Generic;

public class ExampleData : IData
{
	public List<string> words = new List<string>();

	public void Decode(string objectData)
	{
		throw new System.NotImplementedException();
	}

	public string Encode()
	{
		string str = "";
		foreach (string word in words)
		{
			str += word + ";";
		}
		return str;
	}
}
