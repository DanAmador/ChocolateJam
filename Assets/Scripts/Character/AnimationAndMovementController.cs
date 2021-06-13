using System;
using System.Collections;
using Shadow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character {
	[RequireComponent(typeof(PlayerComponent))]
	public class AnimationAndMovementController : MonoBehaviour {
		private PlayerInput _playerInput;
		private CharacterController _characterController;
		private Vector2 _currentMovementInput;
		private Vector3 _currentMovement;

		private PlayerComponent _pc;

		public bool IsMovementPressed => _currentMovement.x != 0 || _currentMovement.y != 0;
		private Vector3 _moveVector;

		#region Unity Lifecycle

		private void Awake() {
			_pc = GetComponent<PlayerComponent>();
			_playerInput = new PlayerInput();
			_characterController = GetComponent<CharacterController>();
			_playerInput.CharacterController.Move.started += OnMovementInput;
			_playerInput.CharacterController.Move.canceled += OnMovementInput;
			_playerInput.CharacterController.Move.performed += OnMovementInput;

			// _playerInput.CharacterController.Spawn.started += OnSpawnInput;
			// _playerInput.CharacterController.Spawn.canceled += OnSpawnInput;
			_playerInput.CharacterController.Spawn.performed += OnSpawnInput;

			_playerInput.CharacterController.Boost.started += OnBoostInput;
			_playerInput.CharacterController.Boost.canceled += OnBoostInput;
		}

		private void Update() {
			_characterController.Move((_characterController.isGrounded ? Vector3.zero : Physics.gravity) +
			                          _currentMovement * (Time.deltaTime * _pc.SpeedWithBoost));
		}

		private void OnEnable() {
			_playerInput.CharacterController.Enable();
		}

		private void OnDisable() {
			_playerInput.CharacterController.Disable();
		}

		#endregion

		#region Input callbacks

		void OnBoostInput(InputAction.CallbackContext ctx) {
			_pc.isBoosting = ctx.ReadValueAsButton();

		}

		void OnSpawnInput(InputAction.CallbackContext ctx) {
#if UNITY_EDITOR
			_pc.SpawnShadow();
#endif
		}

		void OnMovementInput(InputAction.CallbackContext ctx) {
			_currentMovementInput = ctx.ReadValue<Vector2>();
			_currentMovement.x = _currentMovementInput.x;
			_currentMovement.z = _currentMovementInput.y;
		}

		#endregion


	}
}