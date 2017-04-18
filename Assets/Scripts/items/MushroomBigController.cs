using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBigController : ItemBehaviour {

	protected override void SetSprite() {
		sr.sprite = SpriteLoader.objects ["mushroom_big"];
	}

}
