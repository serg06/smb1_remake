using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	private int zDistance = -10;
	private Camera cam;
	private Vector3 offset;
	private Bounds levelBounds;

	private SpriteRenderer sr;

	public void Start() {
		offset = new Vector3 (0, 0, zDistance);
		cam = GetComponent<Camera> ();
		levelBounds = FindObjectOfType<LevelLoader> ().GetComponent<SpriteRenderer> ().sprite.bounds;
	}

	public void FollowPlayer(GameObject player) {
		this.player = player;
	}

	void Update() {
		// follow player
		transform.position = player.transform.position + offset;

		// Send ray towards left edge of camera
		Ray leftView = cam.ScreenPointToRay (new Vector3 (0, cam.pixelHeight/2, 0));

		// Get angle between ray and straight ray
		float angle = Vector2.Angle(new Vector2(leftView.direction.x, leftView.direction.z), new Vector2(0, 1));

		// Get point where ray intersects plane
		float rayLength = Mathf.Abs(zDistance/Mathf.Cos(angle));
		// FUCK IT'S WRONG!
		Vector3 intersectionPoint = leftView.GetPoint (rayLength);

		Debug.DrawRay (leftView.origin, leftView.direction*100, Color.yellow, 0.5f);
		Debug.Log (leftView.direction);
		Debug.Log (intersectionPoint.ToString ());


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
