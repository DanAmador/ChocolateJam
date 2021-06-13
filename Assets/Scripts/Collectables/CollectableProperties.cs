using UnityEngine;

namespace Collectables {
    [CreateAssetMenu(fileName = "CollectableProperties", menuName = "Collectable/CollectableProperties")]
    public class CollectableProperties : ScriptableObject
    {
        [Tooltip("Maximum lifetime in seconds")]
        public float maxLifetime;
        [Tooltip("Chance that the item decays before it reaches its maximum lifetime")]
        public float chanceOfDecay;
        [Tooltip("The duration of the process of decay takes until the item disappears")]
        public float durationOfDecay;
        [Tooltip("Energy that the item provides when used")]
        public int energy;
    }
}
