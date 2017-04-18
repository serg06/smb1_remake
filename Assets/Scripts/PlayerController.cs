using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public int jumpHeight;
	private MovementSprites sprites;

	Rigidbody2D rb;
	SpriteRenderer sr;
	internal BoxCollider2D bc;
	CameraController cam;

	enum marioState {BIG, SMALL};
	marioState state = marioState.SMALL;

	void Start () {
		// get components
		rb = GetComponent<Rigidbody2D> ();
		sr = GetComponent<SpriteRenderer> ();
		bc = GetComponent<BoxCollider2D> ();
		cam = Camera.main.GetComponent<CameraController> ();

		rb.freezeRotation = true;

		// match boundary to sprite
		bc.size = sr.sprite.bounds.size;
		//bc.offset = new Vector2((sr.sprite.bounds.size.x / 2), 0);

		sprites = new MovementSprites ();
		sprites.Start ();
	}

	void Update() {
		float xaxis = Input.GetAxisRaw ("Horizontal");

		// Walking movement using force
		/*
		if (xaxis > 0) {
			rb.AddForce (new Vector2 (xaxis, 0) * (maxSpeed - rb.velocity.x) * speed);
		} else if (xaxis < 0) {
			rb.AddForce (new Vector2 (xaxis, 0) * (maxSpeed + rb.velocity.x) * speed);
		}*/

		// Walking movement using velocity
		if (xaxis > 0) {
			rb.velocity += new Vector2 (Mathf.Min(maxSpeed - rb.velocity.x, maxSpeed) * speed, 0);
		} else if (xaxis < 0) {
			rb.velocity += new Vector2 ((-1) * Mathf.Min(maxSpeed + rb.velocity.x, maxSpeed) * speed, 0);
		}

		// Jump
		if (Input.GetKeyDown (KeyCode.W)) {
			rb.AddForce (new Vector2 (0, 1) * jumpHeight, ForceMode2D.Impulse);
			sr.sprite = sprites.jump ();
		}

		// Walk animations
		if (xaxis == 0) {
			sr.sprite = sprites.stand ();
		} else if (xaxis > 0) {
			sr.sprite = sprites.move ();
			sr.flipX = false;
		} else if (xaxis < 0) {
			sr.sprite = sprites.move();
			sr.flipX = true;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		// if hit big shroom, power up
		if (other.gameObject.name.ToLower ().Contains ("mushroom")) {
			Destroy (other.gameObject);
			powerUp ();
		}

		// if hit ground, stand
		else if (rb.velocity.y < 1) {
			sr.sprite = sprites.land ();
		}
	}

	int morphState;

	void powerUp() {
		if (state == marioState.BIG) {
			return;
		}

		// disable physics n shit
		enabled = false;
		rb.simulated = false;
		cam.enabled = false;

		morphState = 0;
		InvokeRepeating ("growBig", 0, 0.1f);

		// 1 = large
		sprites.type = 1;
		bc.size = new Vector2 (1, 2);
	}

	void growBig() {
		// morph
		if (morphState % 2 == 0) {
			transform.localScale = new Vector2 (1, 1.5f);
			transform.position = new Vector2 (transform.position.x, transform.position.y + 0.25f);
		} else if (morphState % 2 == 1) {
			transform.localScale = new Vector2 (1, 1);
			transform.position = new Vector2 (transform.position.x, transform.position.y - 0.25f);
		}

		// if done morphing
		if (morphState == 7) {
			CancelInvoke ();
			bc.size = new Vector2 (1, 2);
			sprites.type = 1;
			sr.sprite = sprites.stand ();

			// enable physics n shit
			enabled = true;
			rb.simulated = true;
			cam.enabled = true;

			state = marioState.BIG;
		}

		morphState++;
	}

	void takeDamage() {
		if (state == marioState.SMALL) {
			return;
		}

		// 0 = small
		sprites.type = 0;
		bc.size = new Vector2 (1, 1);
	}
}

[System.Serializable]
public class MovementSprites {
	// 0 = mini, 1 = large
	internal int type = 0;

	private Sprite[] standing;
	private Sprite[] jumping;
	private Sprite[][] walking;
	private Sprite[] turning;

	internal bool isJumping = false;
	private float animationSpeed = 0.1f;
	private int walkingIndex = 0;
	private int stepsTaken = 0;

	private float lastStep;

	public void Start() {
		standing = new Sprite[] {
			SpriteLoader.marios ["mario_mini_standing"],
			SpriteLoader.marios ["mario_standing"]
		};
		jumping = new Sprite[] {
			SpriteLoader.marios ["mario_mini_jumping"],
			SpriteLoader.marios ["mario_jumping"]
		};
		walking = new Sprite[][] {
			// mini
			new Sprite[] {
				SpriteLoader.marios ["mario_mini_walking_1"],
				SpriteLoader.marios ["mario_mini_walking_2"],
				SpriteLoader.marios ["mario_mini_walking_3"]
			},

			// large
			new Sprite[] {
				SpriteLoader.marios ["mario_walking_1"],
				SpriteLoader.marios ["mario_walking_2"],
				SpriteLoader.marios ["mario_walking_3"]
			}
		};
		turning = new Sprite[] {
			SpriteLoader.marios ["mario_turning"],
			SpriteLoader.marios ["mario_mini_turning"]
		};

		lastStep = Time.time;
	}

	public Sprite move() {
		if (isJumping) {
			return jumping [type];
		} else {
			if (Time.time - lastStep > (stepsTaken <= 3 ? animationSpeed : animationSpeed/2)) {
				lastStep = Time.time;
				stepsTaken++;
				return walking [type] [walkingIndex = (walkingIndex + 1) % walking.Length];
			} else {
				return walking [type] [walkingIndex];
			}
		}
	}

	public Sprite stand() {
		if (isJumping) {
			return jumping [type];
		} else {
			return forceStand ();
		}
	}

	public Sprite forceStand() {
		isJumping = false;
		walkingIndex = 0;
		stepsTaken = 0;
		return standing [type];
	}

	public Sprite jump() {
		isJumping = true;
		return jumping [type];
	}

	public Sprite land() {
		if (isJumping) {
			isJumping = false;
			return standing [type];
		} else {
			return walking [type] [walkingIndex];
		}
	}

	// todo: implement
	public Sprite turn() {
		lastStep = Time.time;
		return turning [type];
	}
}
