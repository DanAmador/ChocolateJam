using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character {
	public class AnimationAndMovementController : MonoBehaviour {
		private PlayerInput _playerInput;
		private CharacterController _characterController;
		private Vector2 _currentMovementInput;
		private Vector3 _currentMovement;


		public float speed;
		public bool IsMovementPressed => _currentMovement.x != 0 || _currentMovement.y != 0;

		public AnimationAndMovementController(Vector3 currentMovement) {
			this._currentMovement = currentMovement;
		}

		private void Awake() {
			_playerInput = new PlayerInput();
			_characterController = GetComponent<CharacterController>();
			_playerInput.CharacterController.Move.started += OnMovementInput;
			_playerInput.CharacterController.Move.canceled += OnMovementInput;
			_playerInput.CharacterController.Move.performed += OnMovementInput;
		}


		void OnMovementInput(InputAction.CallbackContext ctx) {
			_currentMovementInput = ctx.ReadValue<Vector2>();
			_currentMovement.x = _currentMovementInput.x;
			_currentMovement.z = _currentMovementInput.y;
		}

		private void Update() {
			_characterController.Move(_currentMovement * (Time.deltaTime * speed));
		}

		private void OnEnable() {
			_playerInput.CharacterController.Enable();
		}

		private void OnDisable() {
			_playerInput.CharacterController.Disable();
		}
	}
}