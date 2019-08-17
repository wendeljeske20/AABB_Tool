using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
            str += positions[i + 1].ToString() + " ";
            str += positions[i + 2].ToString() + "\n";


            vertIndex++;


        }
        return str;
    }

    public void SavePointsData()
    {

        StreamWriter pointsFileWriter = new StreamWriter("Assets/Resources/pointsData.txt");

        pointsFileWriter.Write(Encode());
        pointsFileWriter.Close();



    }

    public void LoadPointsData()
    {
        StreamReader pointsFileReader = new StreamReader("Assets/Resources/pointsData.txt");
        string text = pointsFileReader.ReadToEnd();

        if (text != "")
        {
            text = text.Trim();
            string[] splittedText = text.Split(' ', '\n');
            positions.Clear();

            for (int i = 0; i < splittedText.Length; i += 3)
            {
                Vector3 pointPosition = new Vector3(float.Parse(splittedText[i]), float.Parse(splittedText[i + 1]), float.Parse(splittedText[i + 2]));

                positions.Add(pointPosition.x);
                positions.Add(pointPosition.y);
                positions.Add(pointPosition.z);
            }
        }
        pointsFileReader.Close();

    }
}
