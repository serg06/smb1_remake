using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBlockController : MonoBehaviour {

	public Animation sprites;
	private float animationLength = 1;
	private SpriteRenderer sr;
	private GameObject item;
	private bool used = false;

	void Start() {
		sr = GetComponent<SpriteRenderer> ();
		sprites.Start ();
	}

	void Update() {
		if (!used) {
			sr.sprite = sprites.next ();
		} else {
			// if item has been destroyed before it fully went up, done.
			if (item == null) {
				enabled = false;
				return;
			}

			// move item up
			item.transform.position = new Vector2(item.transform.position.x, Mathf.Min(item.transform.position.y + (Time.deltaTime * animationLength), transform.position.y + 1));
			// if it's reached 1 block up, kick it off and disable self
			if (item.transform.position.y == transform.position.y + 1) {
				MushroomBigController itemScript = item.GetComponent<MushroomBigController> ();
				itemScript.enabled = true;
				itemScript.KickOff ();
				// should work but does nothing atm.. oh well, object works anyways
				enabled = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if hit from bottom
		if (!used && other.contacts [0].normal.Equals (new Vector2 (0, 1))) {
			Activate ();
		}
	}

	void Activate() {
		// todo: make mushroom appear
		item = Instantiate(Resources.Load<GameObject>("Objects/mushroom_big"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
		//item.SetActive (false);
		sr.sprite = sprites.use ();
		used = true;
	}
}

[System.Serializable]
public class Animation {

	public Sprite[] stages;
	public Sprite empty;

	private float animationSpeed = 0.2f;
	private float lastStep;
	private int stageIndex = 0;

	public void Start() {
		lastStep = Time.time;
	}

	public Sprite next() {
		if (Time.time - lastStep > animationSpeed) {
			lastStep = Time.time;
			return stages [stageIndex = (stageIndex + 1) % stages.Length];
		}
		return stages [stageIndex];
	}

	public Sprite use() {
		return empty;
	}
}