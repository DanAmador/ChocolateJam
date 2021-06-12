using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shadow {
	[Serializable]
	public struct PathDataFrame {
		public Vector3 position;
		public Vector3 moveDirection;

		public PathDataFrame(Vector3 pos, Vector3 lastDistance) {
			moveDirection = lastDistance.normalized;
			position = pos;
		}
	}
}