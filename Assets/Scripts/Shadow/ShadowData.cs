using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shadow {
	[Serializable]
	public struct PathDataFrame {
		public Vector3 position;
		public Vector3 moveDirection;
		public int amountOfFrames;

		public float TotalTime(float refreshRate) => (float) amountOfFrames * refreshRate;
		public PathDataFrame(Vector3 pos, Vector3 lastDistance) {
			moveDirection = lastDistance.normalized;
			position = pos;
			amountOfFrames = 1;
		}

		// public bool IsDifferentEnough(Transform newPos) => Vector3.Dot(forward, newPos.forward) < .5f ||
		// Vector3.Distance(position, newPos.position) > 1f;
	}


}