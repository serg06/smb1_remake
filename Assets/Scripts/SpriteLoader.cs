using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteLoader : MonoBehaviour {

	public static Dictionary<string, Sprite> marios;
	public static Dictionary<string, Sprite> objects;

	// Use this for initialization
	void Start () {
		enabled = false;
		marios = Resources.LoadAll<Sprite> ("Sprites/marios").ToDictionary (item => item.name);
		objects = Resources.LoadAll<Sprite> ("Sprites/objects").ToDictionary (item => item.name);
	}
}
