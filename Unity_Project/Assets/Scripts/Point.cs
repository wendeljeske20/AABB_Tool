using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIZLab;

public class Point : IData
{

    public List<string> words = new List<string>();
    public Vector3 position;
    
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
