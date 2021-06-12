using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shadow {
	public class ShadowPath : MonoBehaviour {
		private List<PathDataFrame> _frames;

		private float _startTime, _endTime;
		public float Duration => _endTime - _startTime;

		public bool closed;

		public LTSpline spline;

		void Start() {
			_startTime = Time.time;
			_frames = new List<PathDataFrame>();
		}



		public void Add(PathDataFrame lastShadowFrame) {
			_frames.Add(lastShadowFrame);
		}

		public void ClosePath() {
			closed = true;
			_endTime = Time.time;
			spline = new LTSpline(_frames.Select(frame => frame.position).ToArray());
		}

		void OnDrawGizmos() {
			if (spline != null) {
				spline.gizmoDraw(); // debug aid to be able to see the path in the scene inspector
			}
		}
	}
}