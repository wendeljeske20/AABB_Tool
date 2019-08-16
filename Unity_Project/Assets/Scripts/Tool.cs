using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using VIZLab;

public class Tool : BaseTool<PointsData>
{
    public string[] splittedText;
    public float pointSize;
    //public Color pointColor;
    float lowestPointX, greaterPointX, lowestPointY, greaterPointY, lowestPointZ, greaterPointZ;
    public GizmoCamera gizmoCamera;
    private Material material;
    void Start()
    {
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        LoadData();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane groundPlane = new Plane(-Camera.main.transform.forward, gizmoCamera.origin);
            float rayLenght;
            if (groundPlane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointPosition = cameraRay.GetPoint(rayLenght);


                if (data.positions.Count == 0)
                {
                    DefineStartAABBExtension(pointPosition);
                }

                data.positions.Add(pointPosition.x);
                data.positions.Add(pointPosition.y);
                data.positions.Add(pointPosition.z);


                CheckPointExtension(pointPosition);


            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SaveData();

    }


    void SaveData()
    {

        StreamWriter writer = new StreamWriter("Assets/text.txt");

        writer.WriteLine(data.Encode());
        writer.Close();


    }

    void LoadData()
    {
        StreamReader reader = new StreamReader("Assets/text.txt");
        string text = reader.ReadToEnd();

        if (text != "")
        {
            text = text.Trim();
            splittedText = text.Split(' ', '\n');

            for (int i = 0; i < splittedText.Length; i += 3)
            {
                Vector3 pointPosition = new Vector3(float.Parse(splittedText[i]), float.Parse(splittedText[i + 1]), float.Parse(splittedText[i + 2]));

                if (data.positions.Count == 0)
                {
                    DefineStartAABBExtension(pointPosition);
                }


                data.positions.Add(pointPosition.x);
                data.positions.Add(pointPosition.y);
                data.positions.Add(pointPosition.z);



                CheckPointExtension(pointPosition);
            }


        }
    }

    void DefineStartAABBExtension(Vector3 pointPosition)
    {
        lowestPointX = pointPosition.x;
        lowestPointY = pointPosition.y;
        lowestPointZ = pointPosition.z;

        greaterPointX = pointPosition.x;
        greaterPointY = pointPosition.y;
        greaterPointZ = pointPosition.z;
    }

    void CheckPointExtension(Vector3 pointPosition)
    {
        if (pointPosition.x < lowestPointX)
            lowestPointX = pointPosition.x;
        if (pointPosition.x > greaterPointX)
            greaterPointX = pointPosition.x;

        if (pointPosition.y < lowestPointY)
            lowestPointY = pointPosition.y;
        if (pointPosition.y > greaterPointY)
            greaterPointY = pointPosition.y;

        if (pointPosition.z < lowestPointZ)
            lowestPointZ = pointPosition.z;
        if (pointPosition.z > greaterPointZ)
            greaterPointZ = pointPosition.z;

    }

  
    private void OnPostRender()
    {
        material.SetPass(0);

        Vector3[] pointVertices =
        {
            new Vector3(0,0,0), //0
            new Vector3(1,0,0), //1
            new Vector3(1,0,1), //2
            new Vector3(0,0,1), //3
            new Vector3(0,-1,0),//4
            new Vector3(1,-1,0),//5
            new Vector3(1,-1,1),//6
            new Vector3(0,-1,1),//7
        };

        int[,] triangles =
        {
            {0,3,1,2}, //top
            {6,7,5,4}, //bot
            {4,0,5,1}, //front
            {6,2,7,3}, //back
            {7,3,4,0}, //left
            {5,1,6,2}, //right

        };

        for (int i = 0; i < data.positions.Count; i += 3)
        {
            float x = data.positions[i];
            float y = data.positions[i + 1];
            float z = data.positions[i + 2];







            //X Axis

            Vector3 origin = new Vector3(x, y, z);

            GL.wireframe = false;
            for (int j = 0; j < 6; j++)
            {
                GL.PushMatrix();
                GL.Begin(GL.TRIANGLES);
                GL.Color(Color.red);

                GL.Vertex(origin + pointVertices[triangles[j, 0]] * pointSize);

                GL.Vertex(origin + pointVertices[triangles[j, 1]] * pointSize);
                GL.Vertex(origin + pointVertices[triangles[j, 2]] * pointSize);
                GL.Vertex(origin + pointVertices[triangles[j, 2]] * pointSize);
                GL.Vertex(origin + pointVertices[triangles[j, 1]] * pointSize);
                GL.Vertex(origin + pointVertices[triangles[j, 3]] * pointSize);
                GL.End();
                GL.PopMatrix();

            }




        }
        GL.wireframe = true;

        Vector3[] boundingBoxVertices =
        {
            new Vector3(lowestPointX, greaterPointY, lowestPointZ), //0
            new Vector3(greaterPointX, greaterPointY, lowestPointZ), //1
            new Vector3(greaterPointX, greaterPointY, greaterPointZ), //2
            new Vector3(lowestPointX, greaterPointY, greaterPointZ), //3
            new Vector3(lowestPointX, lowestPointY, lowestPointZ), //4
            new Vector3(greaterPointX, lowestPointY, lowestPointZ), //5
            new Vector3(greaterPointX, lowestPointY, greaterPointZ), //6
            new Vector3(lowestPointX, lowestPointY, greaterPointZ), //7
        };

        for (int j = 0; j < 6; j++)
        {
            GL.PushMatrix();
            GL.Begin(GL.TRIANGLES);
            GL.Color(Color.green);

            GL.Vertex(boundingBoxVertices[triangles[j, 0]]);
            GL.Vertex(boundingBoxVertices[triangles[j, 1]]);
            GL.Vertex(boundingBoxVertices[triangles[j, 2]]);
            GL.Vertex(boundingBoxVertices[triangles[j, 2]]);
            GL.Vertex(boundingBoxVertices[triangles[j, 1]]);
            GL.Vertex(boundingBoxVertices[triangles[j, 3]]);
            GL.End();
            GL.PopMatrix();

        }

        GL.wireframe = false;
    }

}
