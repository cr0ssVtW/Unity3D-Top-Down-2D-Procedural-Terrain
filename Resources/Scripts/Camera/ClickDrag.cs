using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDrag : MonoBehaviour {
	
	public int cameraCurrentZoom = 8;
	public int cameraZoomMax = 20;
	public int cameraZoomMin = 5;

	public float dragSpeed = 1.2f;
	private Vector3 dragOrigin;

	private float boundaryMinX;
	private float boundaryMinZ;
	private float boundaryMaxX;
	private float boundaryMaxZ;
	private float boundaryClampX;
	private float boundaryClampZ;

	Vector3 centeredPosition;

	void Start()
	{
		boundaryClampX = TGMap._mapSizeWidth / 2;
		boundaryClampZ = TGMap._mapSizeHeight / 2;
		boundaryMinX = 0 - boundaryClampX;
		boundaryMinZ = 0 - boundaryClampZ;
		boundaryMaxX = TGMap._mapSizeWidth + boundaryClampX;
		boundaryMaxZ = TGMap._mapSizeHeight + boundaryClampZ;

		Camera.main.orthographicSize = cameraCurrentZoom;
		centeredPosition = new Vector3((float)TGMap._mapSizeWidth / 2, 10, (float)TGMap._mapSizeHeight / 2);	
		transform.SetPositionAndRotation(centeredPosition, Camera.main.transform.rotation);
	}

	void Update()
	{
		// Zoom
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (cameraCurrentZoom < cameraZoomMax)
			{
				cameraCurrentZoom += 1;
				Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + 1);
			} 
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (cameraCurrentZoom > cameraZoomMin)
			{
				cameraCurrentZoom -= 1;
				Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1);
			}   
		}
		// Done with zoom.

		// Drag move. Middle mouse
		if (Input.GetMouseButtonDown(2))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(2))
		{
			return;
		}

		if ( transform.position.x > boundaryMinX && transform.position.z > boundaryMinZ &&
			transform.position.x < boundaryMaxX && transform.position.z < boundaryMaxZ) 
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			Vector3 move = new Vector3(-pos.x * dragSpeed, 0, -pos.y * dragSpeed);
			transform.Translate(move, Space.World);  
		}
		else 
		{
			transform.SetPositionAndRotation(centeredPosition, Camera.main.transform.rotation);
		}
	}
}
