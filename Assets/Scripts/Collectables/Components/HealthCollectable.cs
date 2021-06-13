using Character;

namespace Collectables.Components {
	public class HealthCollectable : BaseItem {
		protected override void ApplyCollectable(PlayerComponent player) {
			base.ApplyCollectable(player);
			player.AddHealth();
		}
	}
}