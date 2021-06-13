using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Shadow {
	public class ShadowPath : MonoBehaviour {
		private List<PathDataFrame> _frames;

		public int InterpolationToIndex(float interpolation) => (int) (Mathf.Lerp(0, _frames.Count, interpolation));

		public LTDescr splineDescription;


		public UnityEvent onCompoundPathLoop = new UnityEvent();

		public Vector3[] RemainingOffsets() {
			List<Vector3> remainingPoints = new List<Vector3>();
			int currIdx = InterpolationToIndex(splineDescription.lastVal);
			int modifier = (int) splineDescription.directionLast;
			int endIdx = modifier > 0 ? _frames.Count - 1 : 0;
			while (currIdx != endIdx) {
				remainingPoints.Add(_frames[currIdx].moveDirection);
				currIdx += modifier;
			}

			return remainingPoints.ToArray();
		}

		public float RemainingTime() {
			int modifier = (int) splineDescription.directionLast;
			return (modifier > 0 ? 1 - splineDescription.lastVal : splineDescription.lastVal) * Duration;
		}

		private float _startTime, _endTime;
		public float Duration => _endTime - _startTime;

		public bool closed;

		public LTSpline spline;

		void Awake() {
			_startTime = Time.time;
			_frames = new List<PathDataFrame>();
		}


		public void Add(PathDataFrame lastShadowFrame) {
			_frames.Add(lastShadowFrame);
		}


		public bool ClosePath(float duration = 0, bool runTween = true) {
			if (_frames.Count > 4) {
				closed = true;
				_endTime = Time.time;
				duration = duration == 0 ? Duration : duration;
				spline = new LTSpline(_frames.Select(frame => frame.position).ToArray());
				if (runTween) {
					splineDescription = LeanTween.moveSpline(gameObject, spline.pts, duration)
						.setEase(LeanTweenType.linear)
						.setLoopPingPong()
						.setOrientToPath(true);
				}

				return true;
			}

			return false;
		}

		void OnDrawGizmos() {
			if (spline != null) {
				spline.gizmoDraw(); // debug aid to be able to see the path in the scene inspector
			}
		}

		//Used to calculate compound paths from multiple parents
		public void UpdatePath(Vector3[] newOffsets, float duration) {
			_frames.Clear();
			Vector3 startPoint = transform.position;
			foreach (Vector3 offset in newOffsets) {
				_frames.Add(new PathDataFrame(startPoint, offset));
				startPoint += offset;
			}


			if (ClosePath(duration, false)) {
				splineDescription = LeanTween.moveSpline(gameObject, spline.pts, duration)
					.setEase(LeanTweenType.linear)
					.setLoopPingPong(1)
					.setOrientToPath(true)
					.setOnComplete(onCompoundPathLoop.Invoke);
			}
		}

		public void Reset() {
			_startTime = Time.time;
			if (_frames == null) {
				_frames = new List<PathDataFrame>();
			}
			else {
				_frames.Clear();
			}

			closed = false;
		}
	}
}