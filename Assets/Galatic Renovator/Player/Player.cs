using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.LowLevel;

namespace GR
{
	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour
	{
		// Configurable
		[Header("Player")]
		[SerializeField] private CharacterController characterController;
		[SerializeField] private Transform firstPersonTransform;

		[Header("Movement Properties")]
		[SerializeField] protected float movementSpeed = 10.0f;

		[SerializeField] protected float acceleration = 12.0f;
		[SerializeField] protected float deceleration = 11.0f;

		[Space]
		[SerializeField] protected float airAcceleration = 2.7f;
		[SerializeField] protected float airDeceleration = 11.0f;

		[Space]
		[SerializeField] private float jumpForce = 5.18f;

		[Space]
		[SerializeField] private float gravityForce = 12.4f;
		[SerializeField] protected float rigidbodyPushForce = 1.0f;
		[SerializeField] protected float rigidbodyPushOffset = 0.35f;

		[Header("Camera Properties")]
		[SerializeField] private float lookSensitivity = 1.0f;
		[SerializeField] private float lookMultiplier = 10.0f;
		[SerializeField] private Vector3 lookAngleLimit = new Vector3(85.0f, -1.0f, -1.0f);


		private Camera cam;
		private Vector3 lookAngles;
		private GameInput.PlayerControls input;
		private Vector3 velocity = Vector3.zero;


		public void SetCameraView(Transform viewTransform)
		{
			if (cam == null) cam = Camera.main;

			Transform fpCam = cam.transform;
			fpCam.parent = viewTransform;
			fpCam.localPosition = Vector3.zero;
			fpCam.localRotation = Quaternion.identity;
		}

		private void Start()
		{
			input = new GameInput.PlayerControls();
			if (Camera.main != null)
				SetCameraView(firstPersonTransform);

			input.Enable();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}


		private void Update()
		{
			if (!input.PlayerActions.enabled)
				return;

			// Update player movement
			bool isGrounded = characterController.isGrounded;
			Vector2 moveInput = Vector2.ClampMagnitude(input.PlayerActions.Move.ReadValue<Vector2>(), 1.0f) * movementSpeed;

			Vector3 moveForce = new Vector3(moveInput.x, 0.0f, moveInput.y);
			if (isGrounded && input.PlayerActions.Jump.triggered)
				velocity.y = jumpForce;

			moveForce = cam.transform.TransformDirection(moveForce);
			
			if (!isGrounded)
			{
				float accelerationCoef = moveForce.sqrMagnitude > 0.0f ? airAcceleration : airDeceleration;
				velocity += moveForce * accelerationCoef * Time.deltaTime;
				velocity.y -= gravityForce * Time.deltaTime;
			}
			else
			{
				float accelerationCoef = moveForce.sqrMagnitude > 0.0f ? acceleration : deceleration;
				velocity = Vector3.Lerp(velocity, moveForce, accelerationCoef * Time.deltaTime);
				velocity.y -= gravityForce * Time.deltaTime;
			}

			characterController.Move(velocity * Time.deltaTime);


			// Update player look
			Vector2 lookValue = input.PlayerActions.Look.ReadValue<Vector2>();
			Vector3 lookInput = new Vector3(-lookValue.y, lookValue.x, 0.0f) * Mathf.Clamp(Time.unscaledDeltaTime, 0.0f, 0.16f) * lookMultiplier * lookSensitivity;
			lookAngles += lookInput;

			// Limit look angles
			if (lookAngleLimit.x > 0.0f) lookAngles.x = Mathf.Clamp(lookAngles.x, -lookAngleLimit.x, lookAngleLimit.x);
			if (lookAngleLimit.y > 0.0f) lookAngles.y = Mathf.Clamp(lookAngles.y, -lookAngleLimit.y, lookAngleLimit.y);
			if (lookAngleLimit.z > 0.0f) lookAngles.z = Mathf.Clamp(lookAngles.z, -lookAngleLimit.z, lookAngleLimit.z);

			// Rotate camera
			cam.transform.localEulerAngles = lookAngles;
			cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

			// Rotate player
			Vector3 yRotation = new Vector3(0.0f, lookInput.y, 0.0f);
			transform.rotation *= Quaternion.Euler(yRotation);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!characterController.isGrounded)
			{
				if (hit.moveDirection.y > 0.0f && velocity.y > 0.0f)
					velocity.y = 0.0f;
				//velocity += hit.normal;
			}

			Rigidbody rigid = hit.rigidbody;
			if (!rigid) return;

			Vector3 force = (hit.moveDirection + Vector3.up * rigidbodyPushOffset) * velocity.magnitude * rigidbodyPushForce;
			rigid.AddForceAtPosition(force, hit.point);
		}
	}
}
