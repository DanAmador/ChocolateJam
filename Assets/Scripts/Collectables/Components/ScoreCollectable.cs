using Character;

namespace Collectables.Components {
	public class ScoreCollectable : BaseItem {
		protected override void ApplyCollectable(PlayerComponent player) {
			player.SpawnShadow();
		}
	}
}
