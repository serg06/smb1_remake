using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ItemBehaviour : MonoBehaviour {

	protected Rigidbody2D rb;
	protected SpriteRenderer sr;
	protected BoxCollider2D bc;
	float speed = 3;
	// bit smaller to prevent mario from hitting it when behind block
	float scale = 0.9f;

	protected void Start () {
		enabled = false;

		rb = GetComponent<Rigidbody2D> ();
		sr = GetComponent<SpriteRenderer> ();
		bc = GetComponent<BoxCollider2D> ();

		SetSprite ();

		bc.enabled = false;
		bc.size = new Vector2 (scale, scale);

		// ignore other items
		foreach (ItemBehaviour ib in FindObjectsOfType<ItemBehaviour>()) {
			Physics2D.IgnoreCollision(bc, ib.bc);
		}

	}

	protected abstract void SetSprite ();

	public void KickOff() {
		bc.enabled = true;
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.mass = 3;
		rb.gravityScale = 3;
		rb.velocity = new Vector2 (speed, 0);
		enabled = true;
		
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if hit a non-player object with side, turn around
		if ((other.contacts [0].normal.Equals (new Vector2 (1, 0)) ||
			other.contacts [0].normal.Equals (new Vector2 (-1, 0)))
			&& !other.gameObject.name.Contains ("player")) {
			rb.velocity = new Vector2 ((-1)*speed, rb.velocity.y);
		}
	}

	void LateUpdate() {
		// Keep velocity 3.0
		rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x)*speed, rb.velocity.y);
	}
}
