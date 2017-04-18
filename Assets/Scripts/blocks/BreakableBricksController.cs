using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBricksController : MonoBehaviour {

	public float breakStrength;

	private GameObject brickPiece;

	void Start() {
		brickPiece = Resources.Load<GameObject> ("Objects/brick_piece");
		if (brickPiece == null) {
			Debug.LogError ("Unable to find brick_piece");
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if hit from bottom
		if (other.contacts [0].normal.Equals (new Vector2 (0, 1))) {
			Activate ();
		}
	}

	void Activate() {
		Instantiate (brickPiece, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity).GetComponent<Rigidbody2D> ().AddForce (new Vector2 (1, 3)*breakStrength, ForceMode2D.Impulse);
		Instantiate (brickPiece, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity).GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-1, 3)*breakStrength, ForceMode2D.Impulse);
		Instantiate (brickPiece, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity).GetComponent<Rigidbody2D> ().AddForce (new Vector2 (1, 0)*breakStrength, ForceMode2D.Impulse);
		Instantiate (brickPiece, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity).GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-1, 0)*breakStrength, ForceMode2D.Impulse);

		Destroy (gameObject);
	}
}
