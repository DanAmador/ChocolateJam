using Character;
using UnityEngine;

namespace Collectables.Components {
	public class SpeedCollectable : BaseItem {
		protected override void ApplyCollectable(PlayerComponent player) {
			base.ApplyCollectable(player);
			player.AddSpeed(itemProperties.energy);
		}
	}
}
