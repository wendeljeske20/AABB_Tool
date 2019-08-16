using UnityEngine;

using VIZLab;

public class ExampleTool : BaseTool<ExampleData>
{
	private void Start()
	{
		CreateData();
	}

	private void Update()
	{
		bool changed = false;
		
		// Checking if a key was pressed
		foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(vKey))
			{
				data.words.Add(vKey.ToString());
				changed = true;
			}
		}
		if (changed)
		{
			Debug.Log(data.Encode());
		}
	}
}
