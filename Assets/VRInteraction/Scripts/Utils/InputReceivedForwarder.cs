using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class InputReceivedForwarder : MonoBehaviour 
	{
		public NetworkedCameraRig networkRig;

		private VRInput _input;
		private VRInteractor _interactor;

		// VRInput vars to keep synced
		private bool _triggerPressed;
		private float _triggerPressure;
		private bool _padTopPressed;
		private bool _padLeftPressed;
		private bool _padRightPressed;
		private bool _padBottomPressed;
		public bool _padCentrePressed;
		public bool _padTouched;
		public bool _padPressed;
		public Vector2 _padPosition;
		public bool _gripPressed;
		public bool _menuPressed;
		public bool _axPressed;

		//	VRInteractor vars to keep synced
		private Vector3 _velocity;
		private Vector3 _angularVelocity;

		void Update()
		{
			if (_input == null) _input = GetComponent<VRInput>();
			if (_interactor == null) _interactor = GetComponent<VRInteractor>();

			bool inputIsDirty = false;
			if (_triggerPressed != _input.TriggerPressed) { inputIsDirty = true; _triggerPressed = _input.TriggerPressed; }
			if (_triggerPressure != _input.TriggerPressure) { inputIsDirty = true; _triggerPressure = _input.TriggerPressure; }

			if (_padTopPressed != _input.PadTopPressed) { inputIsDirty = true; _padTopPressed = _input.PadTopPressed; }
			if (_padLeftPressed != _input.PadLeftPressed) { inputIsDirty = true; _padLeftPressed = _input.PadLeftPressed; }
			if (_padRightPressed != _input.PadRightPressed) { inputIsDirty = true; _padRightPressed = _input.PadRightPressed; }
			if (_padBottomPressed != _input.PadBottomPressed) { inputIsDirty = true; _padBottomPressed = _input.PadBottomPressed; }
			if (_padCentrePressed != _input.PadCentrePressed) { inputIsDirty = true; _padCentrePressed = _input.PadCentrePressed; }
			if (_padTouched != _input.PadTouched) { inputIsDirty = true; _padTouched = _input.PadTouched; }
			if (_padPressed != _input.PadPressed) { inputIsDirty = true; _padPressed = _input.PadPressed; }
			if (_padPosition != _input.PadPosition) { inputIsDirty = true; _padPosition = _input.PadPosition; }
			if (_gripPressed != _input.GripPressed) { inputIsDirty = true; _gripPressed = _input.GripPressed; }
			if (_menuPressed != _input.MenuPressed) { inputIsDirty = true; _menuPressed = _input.MenuPressed; }
			if (_axPressed != _input.AXPressed) { inputIsDirty = true; _axPressed = _input.AXPressed; }

			bool interactorIsDirty = false;
			if (_velocity != _interactor.Velocity) { interactorIsDirty = true; _velocity = _interactor.Velocity; }
			if (_angularVelocity != _interactor.AngularVelocity) { interactorIsDirty = true; _angularVelocity = _interactor.AngularVelocity; }

			if (inputIsDirty) networkRig.InputDirty(_input);
			if (interactorIsDirty) networkRig.InteractorDirty(_interactor, _input.LeftHand);
		}

		/*public void InputReceived(string method)
		{
			if (_input == null) _input = GetComponent<VRInput>();
			networkRig.InputReceived(method, _input.LeftHand);
		}*/
	}
}