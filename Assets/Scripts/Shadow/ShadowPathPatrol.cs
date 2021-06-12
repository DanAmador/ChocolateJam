using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Shadow {
	[RequireComponent(typeof(ShadowPath))]
	public class ShadowPathPatrol : MonoBehaviour {
		public ShadowPath pathToFollow;

		public ShadowPathPatrol parent1, parent2;


		private void Start() {
			pathToFollow = GetComponent<ShadowPath>();
		}


		private (Vector3[], float) CalculateParentsMix() {
			Vector3[] remainingPoints1 = parent1.pathToFollow.RemainingOffsets();
			Vector3[] remainingPoints2 = parent2.pathToFollow.RemainingOffsets();

			float remainingTimeAvg = (parent1.pathToFollow.RemainingTime() + parent2.pathToFollow.RemainingTime()) / 2;
			int amountOfElements = Math.Max(remainingPoints1.Length, remainingPoints2.Length);
			Vector3[] newOffsets = new Vector3[amountOfElements];
			for (int i = 0; i < amountOfElements; i++) {
				newOffsets[i] = remainingPoints1.ElementAtOrDefault(i) + remainingPoints2.ElementAtOrDefault(i);
			}

			return (newOffsets, remainingTimeAvg);
		}

		[Button()]
		public void UpdatePathFromParents() {
			if (parent1 != null && parent2 != null) {
				(Vector3[], float) remainingInfo = CalculateParentsMix();
				pathToFollow.UpdatePath(remainingInfo.Item1, remainingInfo.Item2);
			}
		}
	}
}