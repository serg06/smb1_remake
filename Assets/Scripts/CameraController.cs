using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	private int zDistance = -10;
	private Camera cam;
	private Vector3 offset;

	private Plane background;
	private float levelWidth;
	private float levelHeight;
	private Vector3 levelPosition;

	private SpriteRenderer sr;

	public void Start() {
		offset = new Vector3 (0, 0, zDistance);
		cam = GetComponent<Camera> ();

		LevelLoader levelLoader = FindObjectOfType<LevelLoader> ();
		float pixelsPerUnit = levelLoader.GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit;
		levelWidth = levelLoader.width/pixelsPerUnit;
		levelHeight = levelLoader.height/pixelsPerUnit;
		levelPosition = levelLoader.transform.position;

		background = FindObjectOfType<LevelLoader> ().background;
	}

	public void FollowPlayer(GameObject player) {
		this.player = player;
	}

	void Update() {
		// follow player
		transform.position = player.transform.position + offset;

		Vector2 bottomLeftCornerOfMap = new Vector2(levelPosition.x - (levelWidth/2), levelPosition.y - (levelHeight/2));
		Vector2 topRightCornerOfMap = new Vector2(levelPosition.x + (levelWidth/2), levelPosition.y + (levelHeight/2));

		// Send ray towards bottom left and top right corners of camera
		Ray bottomLeftRay = cam.ScreenPointToRay (new Vector2 (0, 0));
		Ray topRightRay = cam.ScreenPointToRay (new Vector2 (cam.pixelWidth, cam.pixelHeight));


		// Get the bottom-left-most visible point
		float rayDistance;
		if (!background.Raycast (bottomLeftRay, out rayDistance)) {
			Debug.LogError ("Camera can't see plane background plane??");
		}
		Vector3 bottomLeftVisiblePoint = bottomLeftRay.GetPoint (rayDistance);

		// Get the top-right-most visible point
		if (!background.Raycast (topRightRay, out rayDistance)) {
			Debug.LogError ("Camera can't see plane background plane??");
		}
		Vector3 topRightVisiblePoint = topRightRay.GetPoint (rayDistance);


		float xDistance, yDistance;

		// Keep top-right-corner in bounds
		xDistance = bottomLeftCornerOfMap.x - bottomLeftVisiblePoint.x;
		yDistance = bottomLeftCornerOfMap.y - bottomLeftVisiblePoint.y;
		Debug.Log ("BOTTOMLEFT: xDistance, yDistance: " + xDistance + "," + yDistance);
		transform.position = new Vector3 (transform.position.x + Mathf.Max (xDistance, 0), transform.position.y + Mathf.Max (yDistance, 0)) + offset;

		// Keep top-right-corner in bounds
		xDistance = topRightCornerOfMap.x - topRightVisiblePoint.x;
		yDistance = topRightCornerOfMap.y - topRightVisiblePoint.y;
		Debug.Log ("TOPRIGHT: xDistance, yDistance: " + xDistance + "," + yDistance);
		transform.position = new Vector3 (transform.position.x - Mathf.Min (xDistance, 0), transform.position.y + Mathf.Min (yDistance, 0)) + offset;


		/*
		// keep camera in bounds
		Vector2 vectorToBottomLeftCornerOfMap = new Vector2 (transform.position.x, transform.position.y) - new Vector2 (0, 0);
		float cameraWideness = Mathf.Abs (Mathf.Tan (Mathf.Deg2Rad * cam.fieldOfView / 2) * zDistance);
		float leftVisibleEdge = transform.position.x - cameraWideness;
		if (leftVisibleEdge < transform.position.x - vectorToBottomLeftCornerOfMap.x) {
			transform.position = new Vector2 (vectorToBottomLeftCornerOfMap.x, transform.position.y);
		}

		Debug.Log ("Left visible edge: " + leftVisibleEdge);
		Debug.Log ("Left corner of map: " + (transform.position.x - vectorToBottomLeftCornerOfMap.x));*/
	}

}
