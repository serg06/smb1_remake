using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorToPrefab {
	public Color32 color;
	public GameObject prefab;
}

public class LevelLoader : MonoBehaviour {

	public Texture2D levelMap;
	public ColorToPrefab[] colorToPrefab;
	public CameraController cam;
	internal int width;
	internal int height;
	internal Plane background;

	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		cam = Camera.main.GetComponent<CameraController>();
		EmptyMap ();
		LoadMap ();
		CreateBackPlane ();
	}

	void EmptyMap() {
		while (transform.childCount > 0) {
			Transform c = transform.GetChild (0);
			c.SetParent (null);
			Destroy (c.gameObject);
		}
	}

	// create plane to get camera ray intersections
	void CreateBackPlane() {
		// new plane (normal, point-on-plane)
		background = new Plane(new Vector3(0, 0, 1), new Vector3(1, 1, 0));
	}
	
	void LoadMap() {
		Color32[] allMyPixels = levelMap.GetPixels32 ();

		width = levelMap.width;
		height = levelMap.height;

		Debug.Log ("Map dimensions: " + width + " x " + height + ".");

		for (int x = 0; x < width; x += 16) {
			for (int y = 0; y < height; y += 16) {
				SpawnTileAt (allMyPixels [(y * width) + x], x/16, y/16);
			}
		}
	}

	void SpawnTileAt(Color32 c, int x, int y) {
		if (c.a == 0 || c.r == 255 && c.g == 255 && c.b == 255) {
			return;
		}

		// todo: improve with dictionary lookup
		foreach (ColorToPrefab ctp in colorToPrefab) {
			if (c.Equals(ctp.color)) {
				GameObject go = (GameObject) Instantiate (ctp.prefab, new Vector3 (x, y, 0), Quaternion.identity);
				if (ctp.prefab.name == "player") {
					cam.FollowPlayer (go);
				}
				return;
			}
		}

		Debug.LogError ("Couldn't find matching color prefab for color: " + c.ToString());
	}
}
