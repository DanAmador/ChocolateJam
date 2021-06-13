using Character;
using UnityEngine;

namespace Collectables.Components {
	public class SpeedCollectable : BaseItem {
		protected override void ApplyCollectable(PlayerComponent player) {
			Debug.Log("ay lemao gotta go fast go speed racer, go ");
		}
	}
}
