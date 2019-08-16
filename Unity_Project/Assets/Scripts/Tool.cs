using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VIZLab;

public class Tool : BaseTool<PointsData>
{
    //public string[] splittedText;
    public float pointSize;
    //public Color pointColor;
    float lowestPointX, greaterPointX, lowestPointY, greaterPointY, lowestPointZ, greaterPointZ;
    GizmoCamera gizmoCamera;
    Material material;

    Slider pointSizeSlider, boxRedColorSlider, boxGreenColorSlider, boxBlueColorSlider, boxAlphaColorSlider;
    Toggle wireframeToggle;

    Color boxColor;

    bool canSpawnPoints = true;

    bool wireframe;
    void Start()
    {
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        gizmoCamera = GameObject.Find("Main Camera").GetComponent<GizmoCamera>();

        pointSizeSlider = GameObject.Find("PointSizeSlider").GetComponent<Slider>();
        boxRedColorSlider = GameObject.Find("BoxRedColorSlider").GetComponent<Slider>();
        boxGreenColorSlider = GameObject.Find("BoxGreenColorSlider").GetComponent<Slider>();
        boxBlueColorSlider = GameObject.Find("BoxBlueColorSlider").GetComponent<Slider>();
        boxAlphaColorSlider = GameObject.Find("BoxAlphaColorSlider").GetComponent<Slider>();
        wireframeToggle = GameObject.Find("WireframeToggle").GetComponent<Toggle>();

        CreateData();
        LoadPointsData();
        LoadConfigData();
    }


    void Update()
    {
        pointSize = pointSizeSlider.value;
        wireframe = wireframeToggle.isOn;
        boxColor = new Color(boxRedColorSlider.value, boxGreenColorSlider.value, boxBlueColorSlider.value, boxAlphaColorSlider.value);

        //cria uma plano invisivel na frente da camera e cria pontos com o clique do mouse. A origem do plano fica na posicao do gizmo da camera.
        if (Input.GetMouseButtonDown(0) && canSpawnPoints)
        {

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(-Camera.main.transform.forward, gizmoCamera.origin);
            float rayLenght;

            if (groundPlane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointPosition = cameraRay.GetPoint(rayLenght);


                if (data.positions.Count == 0)
                {
                    DefineStartBoxExtension(pointPosition);
                }

                data.positions.Add(pointPosition.x);
                data.positions.Add(pointPosition.y);
                data.positions.Add(pointPosition.z);


                CheckPointExtension(pointPosition);


            }
        }

        //remove ultimo ponto
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (data.positions.Count > 0)
            {
                data.positions.RemoveAt(data.positions.Count - 1);
                data.positions.RemoveAt(data.positions.Count - 1);
                data.positions.RemoveAt(data.positions.Count - 1);

                if (data.positions.Count > 0)
                {
                    DefineStartBoxExtension(new Vector3(data.positions[0], data.positions[1], data.positions[2]));
                    for (int i = 0; i < data.positions.Count; i += 3)
                    {

                        CheckPointExtension(new Vector3(data.positions[i], data.positions[i + 1], data.positions[i + 2]));
                    }
                }
            }
        }



    }



    void DefineStartBoxExtension(Vector3 pointPosition)
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
            new Vector3(-1,1,-1), //0
            new Vector3(1,1,-1), //1
            new Vector3(1,1,1), //2
            new Vector3(-1,1,1), //3
            new Vector3(-1,-1,-1),//4
            new Vector3(1,-1,-1),//5
            new Vector3(1,-1,1),//6
            new Vector3(-1,-1,1),//7
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


            Vector3 origin = new Vector3(x, y, z);


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

        GL.wireframe = wireframe;

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
            GL.Color(boxColor);

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

    public void Clear()
    {
        data.positions.Clear();
        DefineStartBoxExtension(Vector3.zero);
    }
    public void SavePointsData()
    {

        StreamWriter pointsFileWriter = new StreamWriter("Assets/Resources/pointsData.txt");

        pointsFileWriter.Write(data.Encode());
        pointsFileWriter.Close();



    }

    public void SaveConfigData()
    {
        StreamWriter configFileWriter = new StreamWriter("Assets/Resources/configData.txt");

        configFileWriter.WriteLine(wireframe ? "1" : "0");
        configFileWriter.WriteLine(pointSize.ToString());
        configFileWriter.WriteLine(boxColor.r + " " + boxColor.g + " " + boxColor.b + " " + boxColor.a);
        configFileWriter.Close();
    }

    public void LoadPointsData()
    {
        StreamReader pointsFileReader = new StreamReader("Assets/Resources/pointsData.txt");
        string text = pointsFileReader.ReadToEnd();

        if (text != "")
        {
            text = text.Trim();
            string[] splittedText = text.Split(' ', '\n');
            data.positions.Clear();
            for (int i = 0; i < splittedText.Length; i += 3)
            {
                Vector3 pointPosition = new Vector3(float.Parse(splittedText[i]), float.Parse(splittedText[i + 1]), float.Parse(splittedText[i + 2]));

                if (data.positions.Count == 0)
                {
                    DefineStartBoxExtension(pointPosition);
                }


                data.positions.Add(pointPosition.x);
                data.positions.Add(pointPosition.y);
                data.positions.Add(pointPosition.z);



                CheckPointExtension(pointPosition);
            }


        }
        pointsFileReader.Close();

    }

    public void LoadConfigData()
    {
        StreamReader configFileReader = new StreamReader("Assets/Resources/configData.txt");

        string text = configFileReader.ReadToEnd();

        if (text != "")
        {
            text = text.Trim();
            string[] splittedText = text.Split(' ', '\n');

            if (splittedText[0].Trim() == "1")
                wireframe = true;
            else
                wireframe = false;


            float.TryParse(splittedText[1], out pointSize);
            float.TryParse(splittedText[2], out boxColor.r);
            float.TryParse(splittedText[3], out boxColor.g);
            float.TryParse(splittedText[4], out boxColor.b);
            float.TryParse(splittedText[5], out boxColor.a);

            pointSizeSlider.value = pointSize;
            wireframeToggle.isOn = wireframe;

            boxRedColorSlider.value = boxColor.r;
            boxGreenColorSlider.value = boxColor.g;
            boxBlueColorSlider.value = boxColor.b;
            boxAlphaColorSlider.value = boxColor.a;
        }
        configFileReader.Close();
    }

    public void EnableClick(bool enable)
    {
        canSpawnPoints = enable;
    }
}
