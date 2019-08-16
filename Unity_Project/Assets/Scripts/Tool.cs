using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIZLab;

public class Tool : BaseTool<Point>
{
    public float pointRadius;
    //public Color pointColor;
    List<Point> pointList = new List<Point>();
    float lowestPointX, greaterPointX, lowestPointY, greaterPointY, lowestPointZ, greaterPointZ;
    public GizmoCamera gizmoCamera;
    void Start()
    {

    }


    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane groundPlane = new Plane(-Camera.main.transform.forward, gizmoCamera.origin);
            float rayLenght;
            if (groundPlane.Raycast(cameraRay, out rayLenght))
            {
                Vector3 pointPosition = cameraRay.GetPoint(rayLenght);





                Point point = new Point();
                point.position = pointPosition;
                if (pointList.Count == 0)
                {
                    lowestPointX = point.position.x;
                    lowestPointY = point.position.y;
                    lowestPointZ = point.position.z;

                    greaterPointX = point.position.x;
                    greaterPointY = point.position.y;
                    greaterPointZ = point.position.z;
                }

                pointList.Add(point);

                CheckPointExtension(point);
            }

        }
    }

    void CheckPointExtension(Point point)
    {
        if (point.position.x < lowestPointX)
            lowestPointX = point.position.x;
        if (point.position.x > greaterPointX)
            greaterPointX = point.position.x;

        if (point.position.y < lowestPointY)
            lowestPointY = point.position.y;
        if (point.position.y > greaterPointY)
            greaterPointY = point.position.y;

        if (point.position.z < lowestPointZ)
            lowestPointZ = point.position.z;
        if (point.position.z > greaterPointZ)
            greaterPointZ = point.position.z;

    }

    private void OnDrawGizmos()
    {
        foreach (Point point in pointList)
        {
            Gizmos.DrawWireSphere(point.position, pointRadius);
        }

        Gizmos.DrawWireCube(new Vector3((lowestPointX + greaterPointX) / 2, (lowestPointY + greaterPointY) / 2, (lowestPointZ + greaterPointZ) / 2),
        new Vector3(Mathf.Abs(lowestPointX - greaterPointX), Mathf.Abs(lowestPointY - greaterPointY), Mathf.Abs(lowestPointZ - greaterPointZ)));
    }
}
