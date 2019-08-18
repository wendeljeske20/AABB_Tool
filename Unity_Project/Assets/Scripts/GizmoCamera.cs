using UnityEngine;

namespace VIZLab
{
	[RequireComponent(typeof(Camera))]
	public class GizmoCamera : MonoBehaviour
	{

		[Header("Configurations")]
		/// <summary>
		/// Camera's initial origin distance from GameObject.
		/// </summary>
		[Tooltip("Distance at which the gizmo will be " +
			"instantiated, from Camera's Game Object position.")]
		[Range(0.0f, 100.0f)]
		public float initialOriginDistance = 10.0f;

		/// <summary>
		/// Gizmo initial relative scale.
		/// </summary>
		[Tooltip("Scale for the Gizmo arrows.")]
		[Range(0.0f, 5.0f)]
		public float gizmoScale = 1.0f;

		[Tooltip("Camera Rotation sensitivity.")]
		[Range(0.01f, 10.0f)]
		public float rotSensitivity = 1.0f;

		[Tooltip("Camera Movement sensitivity.")]
		[Range(0.01f, 10.0f)]
		public float moveSensitivity = 1.0f;

		/// <summary>
		/// Reference to Camera component.
		/// </summary>
		private new Camera camera;

		/// <summary>
		/// Gizmo feedback material.
		/// </summary>
		private Material material;

		/// <summary>
		/// Angle of the camera on given axis.
		/// </summary>
		private float hAngle, vAngle;

		/// <summary>
		/// Distance of the camera to center.
		/// </summary>
		private float distance;

		/// <summary>
		/// Gizmo current scale.
		/// </summary>
		private float gizmoSize;

		/// <summary>
		/// Camera origin, where Gizmo is located.
		/// </summary>
		public Vector3 origin;

		/// <summary>
		/// Intructions enabled flag.
		/// </summary>
		private bool guiInstructions = true;

		/// <summary>
		/// Awake sets initial origin, calculates distance,
		/// vertical and horizontal angles. Also sets the
		/// default gizmo material.
		/// </summary>
		private void Awake()
		{
			camera = GetComponent<Camera>();
			material = new Material(Shader.Find("Hidden/Internal-Colored"));

			//Default scale is 1.0f at distance 10.0f
			distance = initialOriginDistance;
			gizmoSize = gizmoScale * (distance * 0.10f);

			Vector3 cameraDir = transform.rotation * Vector3.forward;
			Vector3 squashedDir = new Vector3(cameraDir.x, 0, cameraDir.z);
			origin = transform.position + cameraDir * distance;
			hAngle = Vector3.SignedAngle(Vector3.forward, -squashedDir, Vector3.up);
			vAngle = Vector3.Angle(-squashedDir, -cameraDir);
		}

		/// <summary>
		/// Update process logic only, visual feedback at OnPostRender.
		/// </summary>
		private void Update()
		{
			//Process inputs
			if (Input.GetMouseButton(1)) //Right Button
			{
				//Calculate Rotation
				hAngle += Input.GetAxis("Mouse X") * 180.0f * Time.deltaTime * rotSensitivity;
				vAngle -= Input.GetAxis("Mouse Y") * 180.0f * Time.deltaTime * rotSensitivity;
				vAngle = Mathf.Clamp(vAngle, -89.9f, 89.9f);
				Cursor.visible = false;
				guiInstructions = false;
			}
			else
			{
				Cursor.visible = true;
			}

			if (Input.GetMouseButton(2)) //Middle Button
			{
				//Calculate Movement
				float halfFovTanDistDelta = distance * Time.deltaTime *
					Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f;
				float hMove = halfFovTanDistDelta * -Input.GetAxis("Mouse X") * moveSensitivity;
				float vMove = halfFovTanDistDelta * -Input.GetAxis("Mouse Y") * moveSensitivity;
				origin += transform.rotation * Vector3.right * hMove;
				origin += transform.rotation * Vector3.up * vMove;
				guiInstructions = false;
			}

			//Perform Zooming based on unit scale
			distance -= Input.mouseScrollDelta.y * Mathf.Pow(0.9f, -Mathf.Floor(Mathf.Log10(distance)));
			distance = Mathf.Clamp(distance, camera.nearClipPlane * 1.15f, camera.farClipPlane * 0.85f);

			//Calculate origin to camera vector
			Vector3 toCamera = Vector3.forward;
			Quaternion rotToCam = Quaternion.AngleAxis(hAngle, Vector3.up) *
				Quaternion.AngleAxis(vAngle, Vector3.left);
			toCamera = rotToCam * toCamera;
			toCamera *= distance;

			transform.localRotation = Quaternion.LookRotation(-toCamera, Vector3.up);
			transform.position = origin + toCamera;
		}

		private void OnGUI()
		{
			if (guiInstructions)
			{
				GUI.Box(new Rect(Screen.width * 0.5f - 120, Screen.height * 0.5f - 60, 240, 120), "");
				Rect rect = new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f - 40, 200, 20);
				rect.position += Vector2.right * 35;
				GUI.Label(rect, "Camera Instructions");
				rect.position += Vector2.up * 20 - Vector2.right * 20;
				GUI.Label(rect, "Right Mouse Button - Rotates");
				rect.position += Vector2.up * 20;
				GUI.Label(rect, "Middle Mouse Button - Move");
				rect.position += Vector2.up * 20;
				GUI.Label(rect, "Move or Rotate to close this");
			}
		}

		/// <summary>
		/// GL calls must be executed with OnPostRender.
		/// </summary>
		private void OnPostRender()
		{
			material.SetPass(0);
			GL.PushMatrix();
			GL.LoadProjectionMatrix(camera.projectionMatrix);
			GL.Begin(GL.LINES);

			//X Axis
			GL.Color(Color.red);
			GL.Vertex(origin);
			GL.Vertex(origin + Vector3.right * gizmoSize);

			//Y Axis
			GL.Color(Color.green);
			GL.Vertex(origin);
			GL.Vertex(origin + Vector3.up * gizmoSize);

			//Z Axis
			GL.Color(Color.blue);
			GL.Vertex(origin);
			GL.Vertex(origin + Vector3.forward * gizmoSize);

			GL.End();
			GL.PopMatrix();
		}
	}
}
