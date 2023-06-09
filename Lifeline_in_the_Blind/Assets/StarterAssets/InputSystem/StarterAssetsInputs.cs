using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;
		public bool inventoryToggle;
		public bool radioToggle;
		public bool use;
		public bool StartGame;
		public bool LoadLastCheckpoint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}

		public void OnInventoryToggle(InputValue value)
		{
			InventoryToggleInput(value.isPressed);
		}

		public void OnRadioToggle(InputValue value)
		{
			RadioToggleInput(value.isPressed);
		}

		public void OnUse(InputValue value)
		{
			UseInput(value.isPressed);
		}

		public void OnStartGame(InputValue value)
		{
			StartGameInput(value.isPressed);
		}

		public void OnLoadLastCheckpoint(InputValue value)
		{
			LoadLastCheckpointInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;
		}

		public void InventoryToggleInput(bool newInventoryToggleState)
		{
			inventoryToggle = newInventoryToggleState;
		}

		public void RadioToggleInput(bool newRadioToggleState)
		{
			radioToggle = newRadioToggleState;
		}

		public void UseInput(bool newUseState)
		{
			use = newUseState;
		}

		public void StartGameInput(bool newStartGameState)
		{
			StartGame = newStartGameState;
		}

		private void LoadLastCheckpointInput(bool newLoadLastCheckpointState)
		{
			LoadLastCheckpoint = newLoadLastCheckpointState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			// commenting this out allows door prefabs to work with mouse, however prevents full rotation of character
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}