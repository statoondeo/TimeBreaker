using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : MonoBehaviour, IService
{
	private PlayerControls PlayerControls;

	private void Awake() => PlayerControls = new PlayerControls();

	private void Start() => PlayerControls.InGame.Position.performed += PositionPerformed;

	private void OnEnable() => PlayerControls.Enable();

	private void OnDisable() => PlayerControls.Disable();

	private void PositionPerformed(InputAction.CallbackContext context) => OnPositionChanged?.Invoke(context.ReadValue<Vector2>());

	public event Action<Vector2> OnPositionChanged;
}

