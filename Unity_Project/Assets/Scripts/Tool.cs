using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VIZLab;

public class Tool : BaseTool<PointsData>
{
    //public string[] splittedText;

    //public Color pointColor;

    GizmoCamera gizmoCamera;
    Material material;



    int pointFaceAmount;
    bool canSpawnPoints = true;
    float lowestPointX, highestPointX, lowestPointY, highestPointY, lowestPointZ, highestPointZ;

    [HideInInspector] public int pointCount;
    [HideInInspector] public bool wireframeMode, quadMode;
    [HideInInspector] public float pointSize;
    [HideInInspector] public Color boxColor;


    void Start()
    {
        // Debug.Log(Application.dataPath);
        // Debug.Log(Application.persistentDataPath);
        // Debug.Log(Application.streamingAssetsPath);
        // Debug.Log(Application.temporaryCachePath);
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        gizmoCamera = GameObject.Find("Main Camera").GetComponent<GizmoCamera>();

        CreateData();
        LoadPointsData();

    }


    void Update()
    {
        pointFaceAmount = quadMode ? 1 : 6;
        pointCount = data.positions.Count / 3;

        //cria uma plano invisivel na frente da camera e cria pontos com o clique do mouse. 
        //A origem do plano fica na posicao do gizmo da camera.
        if (Input.GetMouseButtonDown(0) && canSpawnPoints)
        {

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(-Camera.main.transform.forward, gizmoCamera.origin);
            float rayLenght;

            if (plane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointPosition = cameraRay.GetPoint(rayLenght);


                if (data.positions.Count == 0)
                {
                    ResetBoundingBoxDimensions(pointPosition);
                }

                data.positions.Add(pointPosition.x);
                data.positions.Add(pointPosition.y);
                data.positions.Add(pointPosition.z);


                CheckPoint(pointPosition);


            }
        }

        //remove ultimo ponto
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (data.positions.Count >= 3)
            {
                data.positions.RemoveAt(data.positions.Count - 1);
                data.positions.RemoveAt(data.positions.Count - 1);
                data.positions.RemoveAt(data.positions.Count - 1);

                if (data.positions.Count >= 3)
                {
                    ResetBoundingBoxDimensions(new Vector3(data.positions[0], data.positions[1], data.positions[2]));
                    for (int i = 0; i < data.positions.Count; i += 3)
                    {

                        CheckPoint(new Vector3(data.positions[i], data.positions[i + 1], data.positions[i + 2]));
                    }
                }
            }
        }



    }




    void ResetBoundingBoxDimensions(Vector3 pointPosition)
    {
        lowestPointX = pointPosition.x;
        lowestPointY = pointPosition.y;
        lowestPointZ = pointPosition.z;

        highestPointX = pointPosition.x;
        highestPointY = pointPosition.y;
        highestPointZ = pointPosition.z;
    }

    void CheckPoint(Vector3 pointPosition)
    {
        if (pointPosition.x < lowestPointX)
            lowestPointX = pointPosition.x;
        if (pointPosition.x > highestPointX)
            highestPointX = pointPosition.x;

        if (pointPosition.y < lowestPointY)
            lowestPointY = pointPosition.y;
        if (pointPosition.y > highestPointY)
            highestPointY = pointPosition.y;

        if (pointPosition.z < lowestPointZ)
            lowestPointZ = pointPosition.z;
        if (pointPosition.z > highestPointZ)
            highestPointZ = pointPosition.z;

    }


    private void OnPostRender()
    {
        material.SetPass(0);

        int[,] triangles =
        {
            {4,0,5,1}, //front
            {6,2,7,3}, //back
            {0,3,1,2}, //top
            {6,7,5,4}, //bot
            {7,3,4,0}, //left
            {5,1,6,2}, //right

        };

        if (data.positions.Count >= 3)
        {
            DrawPoints(triangles);
            DrawBoundingBox(triangles);
        }


        GL.wireframe = false;
    }

    void DrawPoints(int[,] triangles)
    {
        Vector3[] vertices =
        {
            new Vector3(-1,1,-1), //0
            new Vector3(1,1,-1), //1
            new Vector3(1,1,1), //2
            new Vector3(-1,1,1), //3
            new Vector3(-1,-1,-1), //4
            new Vector3(1,-1,-1), //5
            new Vector3(1,-1,1), //6
            new Vector3(-1,-1,1), //7
        };

        //rotaciona os pontos de acordo com a visao da camera
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Camera.main.transform.rotation * vertices[i];
        }

        for (int i = 0; i < data.positions.Count; i += 3)
        {
            Vector3 origin = new Vector3(data.positions[i], data.positions[i + 1], data.positions[i + 2]);

            for (int j = 0; j < pointFaceAmount; j++)
            {
                GL.Begin(GL.TRIANGLES);
                GL.Color(new Color(0, 1, 1));

                GL.Vertex(origin + vertices[triangles[j, 0]] * pointSize);
                GL.Vertex(origin + vertices[triangles[j, 1]] * pointSize);
                GL.Vertex(origin + vertices[triangles[j, 2]] * pointSize);
                GL.Vertex(origin + vertices[triangles[j, 2]] * pointSize);
                GL.Vertex(origin + vertices[triangles[j, 1]] * pointSize);
                GL.Vertex(origin + vertices[triangles[j, 3]] * pointSize);
                GL.End();

            }

        }
    }

    void DrawBoundingBox(int[,] triangles)
    {
        GL.wireframe = wireframeMode;

        Vector3[] vertices =
        {
            new Vector3(lowestPointX, highestPointY, lowestPointZ), //0
            new Vector3( highestPointX, highestPointY, lowestPointZ), //1
            new Vector3( highestPointX, highestPointY, highestPointZ), //2
            new Vector3(lowestPointX, highestPointY, highestPointZ), //3
            new Vector3(lowestPointX, lowestPointY, lowestPointZ), //4
            new Vector3( highestPointX, lowestPointY, lowestPointZ), //5
            new Vector3( highestPointX, lowestPointY, highestPointZ), //6
            new Vector3(lowestPointX, lowestPointY, highestPointZ), //7
        };

        for (int j = 0; j < 6; j++)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(boxColor);

            GL.Vertex(vertices[triangles[j, 0]]);
            GL.Vertex(vertices[triangles[j, 1]]);
            GL.Vertex(vertices[triangles[j, 2]]);
            GL.Vertex(vertices[triangles[j, 2]]);
            GL.Vertex(vertices[triangles[j, 1]]);
            GL.Vertex(vertices[triangles[j, 3]]);
            GL.End();

        }
    }

    public void ClearPoints()
    {
        data.positions.Clear();
        ResetBoundingBoxDimensions(Vector3.zero);
    }
    public void SavePointsData()
    {
        data.SavePointsData();
    }

    public void LoadPointsData()
    {

        data.LoadPointsData();

        for (int i = 0; i < data.positions.Count; i += 3)
        {
            Vector3 pointPosition = new Vector3(data.positions[i], data.positions[i + 1], data.positions[i + 2]);

            if (i == 0)
            {
                ResetBoundingBoxDimensions(pointPosition);
            }

            CheckPoint(pointPosition);
        }


    }


    public void EnableClick(bool enable)
    {
        canSpawnPoints = enable;
    }
}
