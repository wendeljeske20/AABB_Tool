using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIZLab;

public class PointsData : IData
{

    // public List<string> words = new List<string>();
    public List<float> positions = new List<float>();

    public void Decode(string objectData)
    {
        throw new System.NotImplementedException();
    }

    public string Encode()
    {
        string str = "";
        int vertIndex = 0;
        for (int i = 0; i < positions.Count; i += 3)
        {

           // str += "v ";
            str += positions[i].ToString() + " ";
            str += positions[i + 1].ToString()+ " ";
            str +=  positions[i + 2].ToString() + "\n";
           

            vertIndex++;


        }
        return str;
    }
}
