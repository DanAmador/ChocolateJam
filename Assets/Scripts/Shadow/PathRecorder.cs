using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Shadow {
	[RequireComponent(typeof(ShadowPath))]
	public class PathRecorder : MonoBehaviour {
		public ShadowPath shadowPath;

		[Label("Seconds to wait before recording"), Range(0, 1)]
		public float refreshRate = (10f / 60f);

		private float _timeSinceLastRecord = 0;


		private bool _recording = true;


		[SerializeField] private PathDataFrame lastShadowFrame;

		private Vector3 _lastPosition, _lastDirection;
		
		[Range(0.2f, 2f),SerializeField] private double minSaveDistance;

		#region Unity Loop

		void Start() {
			shadowPath = GetComponent<ShadowPath>();
		}

		void Update() {
			if (_recording) {
				_timeSinceLastRecord += Time.deltaTime;
				if (_timeSinceLastRecord > refreshRate) {
					SaveFrame();
					_timeSinceLastRecord = 0;
				}
			}
		}

		#endregion

		#region Logic

		private bool CanSaveCurrentPosition => Vector3.Distance(transform.position, _lastPosition) > minSaveDistance;

		private void SaveFrame() {
			if ( CanSaveCurrentPosition) {
				Vector3 t = transform.position;
				_lastDirection = t - _lastPosition;
				_lastPosition = t;
				lastShadowFrame = new PathDataFrame(t, _lastDirection);
				shadowPath.Add(lastShadowFrame);
			}
			else {
				lastShadowFrame.amountOfFrames++;
			}
		}

		#endregion


		[Button()]
		public void StopRecording() {
			SaveFrame();
			
			shadowPath.ClosePath();
			_recording = false;
		}
	}
}