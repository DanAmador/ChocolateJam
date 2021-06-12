using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Shadow {
	[RequireComponent(typeof(ShadowPath))]
	public class ShadowPathPatrol : MonoBehaviour {
		public ShadowPath pathToFollow;
		private PathDataFrame currentFrame;

		public int currentFrameIdx;
		public bool forwards;
		public IEnumerator<(PathDataFrame, int, bool)> currentIterator;
		private float _timeSinceLastSample;
		private float _iter;

		private LTDescr _splineDescription;
		private void Start() {
			pathToFollow = GetComponent<ShadowPath>();
		}


		[Button()]
		private void SetupRoute() {
			// forwards = !forwards;

			// currentIterator = pathToFollow.FrameIterator(currentFrameIdx, forwards).GetEnumerator();

			_splineDescription = LeanTween.moveSpline(gameObject, pathToFollow.spline.pts, pathToFollow.Duration).setEase(LeanTweenType.easeInOutQuad)
				.setLoopPingPong().setOrientToPath(true);
		}


		void Move() {
			// Iterating over path
			
			// transform.position = pathToFollow.spline.point(_iter);

			
			_iter += (forwards ? 1 : -1) * Time.deltaTime * 0.1f;
			if (_iter > 1.0f) {
				_iter = 0.0f;
			}
		}

		void Update() {
			if (pathToFollow.closed) {
				Move();
			}
		}
	}
}