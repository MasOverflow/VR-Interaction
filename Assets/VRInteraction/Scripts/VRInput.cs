//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Base class to processes controller input for both Vive and Oculus
// Uses SendMessage to broadcast actions to any attached scripts.
// This script is abstract and can be inherited from by a vr system:
// e.g. SteamVR or Oculus Native
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
#if Int_SteamVR
using Valve.VR;
#endif

namespace VRInteraction
{
	public class VRInput : MonoBehaviour
	{
		public enum HMDType
		{
			VIVE,
			OCULUS
		}

		public string[] VRActions;

		//Used in the editor
		public bool mirrorControls = true;
		// Will display oculus buttons when false
		public bool displayViveButtons;

		public int triggerKey;
		public int padTop;
		public int padLeft;
		public int padRight;
		public int padBottom;
		public int padCentre;
		public int padTouch;
		public int gripKey;
		public int menuKey;
		public int AXKey;

		//Oculus alternative buttons
		public int triggerKeyOculus;
		public int padTopOculus;
		public int padLeftOculus;
		public int padRightOculus;
		public int padBottomOculus;
		public int padCentreOculus;
		public int padTouchOculus;
		public int gripKeyOculus;
		public int menuKeyOculus;
		public int AXKeyOculus;

		private bool _triggerPressedFlag = false;
		private bool _padPressedFlag = false;
		private bool _padTouchedFlag = false;
		private bool _grippedFlag = false;
		private bool _menuPressedFlag = false;
		private bool _AX_PressedFlag = false;

		private bool _stickLeftDown;
		private bool _stickTopDown;
		private bool _stickBottomDown;
		private bool _stickRightDown;

		virtual protected void Start()
		{
			#if Int_Oculus
			if (!isSteamVR())
			{
				bool leftHand = LeftHand; //Assigns LTouch and RTouch if unassigned
			}
			#endif
		}
			
		virtual protected void Update()
		{
			bool trigger = TriggerPressed;
			if (trigger && !_triggerPressedFlag)
			{
				_triggerPressedFlag = true;
				TriggerClicked();
			} else if (!trigger && _triggerPressedFlag)
			{
				_triggerPressedFlag = false;
				TriggerReleased();
			}

			bool thumbstick = PadPressed;
			if (thumbstick && !_padPressedFlag)
			{
				_padPressedFlag = true;
				TrackpadDown();
			} else if (!thumbstick && _padPressedFlag)
			{
				_padPressedFlag = false;
				TrackpadUp();
			}

			bool thumbstickTouch = PadTouched;
			if (thumbstickTouch && !_padTouchedFlag)
			{
				_padTouchedFlag = true;
				TrackpadTouch();
			} else if (!thumbstickTouch && _padTouchedFlag)
			{
				_padTouchedFlag = false;
				_stickLeftDown = false;
				_stickTopDown = false;
				_stickBottomDown = false;
				_stickRightDown = false;
				TrackpadUnTouch();
			}

			if (hmdType == HMDType.OCULUS && _padTouchedFlag)
			{
				if (PadLeftPressed && !_stickLeftDown)
				{
					_stickLeftDown = true;
					SendMessage("InputReceived", VRActions[padLeftOculus], SendMessageOptions.DontRequireReceiver);
				} else if (!PadLeftPressed && _stickLeftDown)
					_stickLeftDown = false;

				if (PadRightPressed && !_stickRightDown)
				{
					_stickRightDown = true;
					SendMessage("InputReceived", VRActions[padRightOculus], SendMessageOptions.DontRequireReceiver);
				} else if (!PadRightPressed && _stickRightDown)
					_stickRightDown = false;

				if (PadBottomPressed && !_stickBottomDown)
				{
					_stickBottomDown = true;
					SendMessage("InputReceived", VRActions[padBottomOculus], SendMessageOptions.DontRequireReceiver);
				} else if (!PadBottomPressed && _stickBottomDown)
					_stickBottomDown = false;

				if (PadTopPressed && !_stickTopDown)
				{
					_stickTopDown = true;
					SendMessage("InputReceived", VRActions[padTopOculus], SendMessageOptions.DontRequireReceiver);
				} else if (!PadTopPressed && _stickTopDown)
					_stickTopDown = false;
			}

			bool grip = GripPressed;
			if (grip && !_grippedFlag)
			{
				_grippedFlag = true;
				Gripped();
			} else if (!grip && _grippedFlag)
			{
				_grippedFlag = false;
				UnGripped();
			}

			bool menu = MenuPressed;
			if (menu && !_menuPressedFlag)
			{
				_menuPressedFlag = true;
				MenuClicked();
			} else if (!menu && _menuPressedFlag)
			{
				_menuPressedFlag = false;
				MenuReleased();
			}

			bool AX = AXPressed;
			if (AX && !_AX_PressedFlag)
			{
				_AX_PressedFlag = true;
				AXClicked();
			} else if (!AX && _AX_PressedFlag)
			{
				_AX_PressedFlag = false;
				AXReleased();
			}
		}

		#if Int_SteamVR && !Int_SteamVR2

		//	If you are getting the error "The Type or namespace name 'SteamVR_TrackedController' could not be found."
		//	but you have SteamVR imported it is likely you imported the newest version of SteamVR which is not currently
		//	supported. The latest version that is supported is version 1.2.3 which you can download here:
		//	https://github.com/ValveSoftware/steamvr_unity_plugin/tree/fad02abee8ed45791993e92e420b340f63940aca
		//	Please delete the SteamVR folder and replace with the one from this repo.
		protected SteamVR_TrackedController _controller;

		public SteamVR_TrackedController controller
		{
			get 
			{
				if (_controller == null) _controller = GetComponent<SteamVR_TrackedController>();
				if (_controller == null) _controller = gameObject.AddComponent<SteamVR_TrackedController>();
				return _controller; 
			}
		}

		#endif

		#if Int_Oculus

		public OVRInput.Controller controllerHand;

		#endif

		virtual public bool isSteamVR()
		{
			#if Int_SteamVR && !Int_SteamVR2
			if (GetComponent<SteamVR_TrackedController>() != null || GetComponent<SteamVR_TrackedObject>() != null || GetComponentInParent<SteamVR_ControllerManager>() != null)
				return true;
			else
				return false;
			#elif Int_SteamVR2
			throw new System.Exception("SteamVR 2 is not currently supported. You can download SteamVR 1.2.3 from here:\n" +
				"https://github.com/ValveSoftware/steamvr_unity_plugin/tree/fad02abee8ed45791993e92e420b340f63940aca\n" +
				"Please delete the SteamVR folder and replace with the one from this repo.");
			#elif Int_Oculus
			return false;
			#else
			throw new System.Exception("Requires SteamVR or Oculus Integration asset");
			#endif
		}
		public string[] getVRActions{get { return VRActions; } set { VRActions = value; }}
		
		virtual public HMDType hmdType
		{
			get
			{
			#if Int_SteamVR && !Int_SteamVR2
			if ((GetComponent<SteamVR_TrackedObject>() == null && GetComponentInParent<SteamVR_ControllerManager>() == null) || SteamVR.enabled && SteamVR.instance != null && SteamVR.instance.hmd_TrackingSystemName == "oculus")
				return HMDType.OCULUS; 
			else
				return HMDType.VIVE;
			#elif Int_Oculus
			return HMDType.OCULUS;
			#else
			throw new System.Exception("Requires SteamVR or Oculus Integration asset");
			#endif
			}
		}
		virtual public bool LeftHand
		{
			get 
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					SteamVR_ControllerManager controllerManager = null;
					if (transform.parent != null) controllerManager = transform.parent.GetComponent<SteamVR_ControllerManager>();
					else controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
					if (controllerManager != null) return gameObject == controllerManager.left;
					else
					{
						Debug.LogError("Can't find SteamVR_ControllerManager in scene");
					}
				}
				#endif

				#if Int_Oculus
				if (!isSteamVR())
				{
					if (controllerHand == OVRInput.Controller.None)
					{
						OvrAvatar avatar = GetComponentInParent<OvrAvatar>();
						if (avatar == null)
						{
							if (name.Contains("left"))
								controllerHand = OVRInput.Controller.LTouch;
							else
								controllerHand = OVRInput.Controller.RTouch;
						} else
						{
							if (avatar.ControllerLeft.transform == transform || avatar.HandLeft.transform == transform)
								controllerHand = OVRInput.Controller.LTouch;
							else if (avatar.ControllerRight.transform == transform || avatar.HandRight.transform == transform)
								controllerHand = OVRInput.Controller.RTouch;
						}
					}
					return controllerHand == OVRInput.Controller.LTouch;
				}
				#endif
				return false;
			}
		}

		public bool ActionPressed(string action)
		{
			for(int i=0; i<VRActions.Length; i++)
			{
				if (action == VRActions[i])
				{
					return ActionPressed(i);
				}
			}
			return false;
		}

		public bool ActionPressed(int action)
		{
			if (hmdType == HMDType.VIVE)
			{
				if (triggerKey == action && TriggerPressed)
					return true;
				if (padTop == action && PadTopPressed)
					return true;
				if (padLeft == action && PadLeftPressed)
					return true;
				if (padRight == action && PadRightPressed)
					return true;
				if (padBottom == action && PadBottomPressed)
					return true;
				if (padCentre == action && PadCentrePressed)
					return true;
				if (padTouch == action && PadTouched)
					return true;
				if (menuKey == action && MenuPressed)
					return true;
				if (gripKey == action && GripPressed)
					return true;
				if (AXKey == action && AXPressed)
					return true;
			} else
			{
				if (triggerKeyOculus == action && TriggerPressed)
					return true;
				if (padTopOculus == action && PadTopPressed)
					return true;
				if (padLeftOculus == action && PadLeftPressed)
					return true;
				if (padRightOculus == action && PadRightPressed)
					return true;
				if (padBottomOculus == action && PadBottomPressed)
					return true;
				if (padCentreOculus == action && PadCentrePressed)
					return true;
				if (padTouchOculus == action && PadTouched)
					return true;
				if (menuKeyOculus == action && MenuPressed)
					return true;
				if (gripKeyOculus == action && GripPressed)
					return true;
				if (AXKeyOculus == action && AXPressed)
					return true;
			}
			return false;
		}

		virtual public bool TriggerPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					return controller.triggerPressed;
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return TriggerPressure > 0.9f;
				}
				#endif
				return false;
			}
		}
		virtual public float TriggerPressure
		{
			get
			{
			#if Int_SteamVR && !Int_SteamVR2
			if (isSteamVR())
			{
				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
			}
			#endif

			#if Int_Oculus
			if (!isSteamVR())
			{
				return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controllerHand);
			}
			#endif
			return 0f;
			}
		}

		virtual public bool PadTopPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					if (controller.padPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						var device = SteamVR_Controller.Input((int)controller.controllerIndex);
						Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
						if (axis.y > (hmdType == HMDType.VIVE ? 0.4f : 0.8f) &&
							axis.x < axis.y &&
							axis.x > -axis.y)
							return true;
					}
				}
				#endif

				#if Int_Oculus
				if (!isSteamVR())
				{
					Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
					if (axis.y > 0.8f &&
						axis.x < axis.y &&
						axis.x > -axis.y)
						return true;
				}
				#endif
				return false;
			}
		}
		virtual public bool PadLeftPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					if (controller.padPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						var device = SteamVR_Controller.Input((int)controller.controllerIndex);
						Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
						if (axis.x < (hmdType == HMDType.VIVE ? -0.4f : -0.5f) &&
							axis.y > axis.x &&
							axis.y < -axis.x)
							return true;
					}
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
					if (axis.x < -0.5f &&
						axis.y > axis.x &&
						axis.y < -axis.x)
						return true;
				}
				#endif
				return false;
			}
		}
		virtual public bool PadRightPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					if (controller.padPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						var device = SteamVR_Controller.Input((int)controller.controllerIndex);
						Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
						if (axis.x > (hmdType == HMDType.VIVE ? 0.4f : 0.5f) &&
							axis.y < axis.x &&
							axis.y > -axis.x)
							return true;
					}
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
					if (axis.x > 0.5f &&
						axis.y < axis.x &&
						axis.y > -axis.x)
						return true;
				}
				#endif
				return false;
			}
		}
		virtual public bool PadBottomPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					if (controller.padPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						var device = SteamVR_Controller.Input((int)controller.controllerIndex);
						Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
						if ((axis.y < (hmdType == HMDType.VIVE ? -0.4f : -0.8f) &&
							axis.x > axis.y &&
							axis.x < -axis.y))
							return true;
					}
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
					if ((axis.y < -0.8f &&
						axis.x > axis.y &&
						axis.x < -axis.y))
						return true;
				}
				#endif
				return false;
			}
		}
		virtual public bool PadCentrePressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					if (controller.padPressed)
					{
						var device = SteamVR_Controller.Input((int)controller.controllerIndex);
						Vector2 axis = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

						if (axis.y >= -0.4f && axis.y <= 0.4f && axis.x >= -0.4f && axis.x <= 0.4f)
							return true;
					}
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					if (OVRInput.Get(OVRInput.Button.DpadDown, controllerHand))
					{
						Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
						if (axis.y >= -0.4f && axis.y <= 0.4f && axis.x >= -0.4f && axis.x <= 0.4f)
							return true;
					}
				}
				#endif
				return false;
			}
		}
		virtual public bool PadTouched
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					return controller.padTouched;
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, controllerHand);
				}
				#endif
				return false;
			}
		}
		virtual public bool PadPressed
		{
			get 
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					return controller.padPressed;
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, controllerHand);
				}
				#endif
				return false;
			}
		}
		virtual public Vector2 PadPosition
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					var device = SteamVR_Controller.Input((int)controller.controllerIndex);
					return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controllerHand);
				}
				#endif
				return Vector2.zero;
			}
		}
		virtual public bool GripPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					return controller.gripped;
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controllerHand) > 0.9f;
				}
				#endif
				return false;
			}
		}
		virtual public bool MenuPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					return controller.menuPressed;
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Button.Two, controllerHand);
				}
				#endif
				return false;
			}
		}
		virtual public bool AXPressed
		{
			get
			{
				#if Int_SteamVR && !Int_SteamVR2
				if (isSteamVR())
				{
					var system = OpenVR.System;
					if (system != null && system.GetControllerState(controller.controllerIndex, ref controller.controllerState, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t))))
					{
						ulong AButton = controller.controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_A));
						return AButton > 0L;
					}
				}
				#endif
				#if Int_Oculus
				if (!isSteamVR())
				{
					return OVRInput.Get(OVRInput.Button.One, controllerHand);
				}
				#endif
				return false;
			}
		}

		public bool isTriggerPressed { get { return _triggerPressedFlag; } }
		public bool isPadPressed { get { return _padPressedFlag; } }
		public bool isPadTouched { get { return _padTouchedFlag; } }
		public bool isGripped { get { return _grippedFlag; } }
		public bool isBY_Pressed { get { return _menuPressedFlag; } }
		public bool isAX_Pressed { get { return _AX_PressedFlag; } }

		virtual protected void SendMessageToInteractor(string message)
		{
			SendMessage("InputReceived", message, SendMessageOptions.DontRequireReceiver);
		}

		protected void TriggerClicked()
		{
			int triggerKey = this.triggerKey;
			if (hmdType == HMDType.OCULUS) triggerKey = this.triggerKeyOculus;
			if (triggerKey >= VRActions.Length)
			{
				Debug.LogWarning("Trigger key index (" + triggerKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[triggerKey]);
		}

		protected void TriggerReleased()
		{
			int triggerKey = this.triggerKey;
			if (hmdType == HMDType.OCULUS) triggerKey = this.triggerKeyOculus;
			if (triggerKey >= VRActions.Length)
			{
				Debug.LogWarning("Trigger key index (" + triggerKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[triggerKey]+"Released");
		}

		protected void TrackpadDown()
		{
			int action = 0;
			if (hmdType == HMDType.VIVE)
			{
				if (PadTopPressed) action = padTop;
				else if (PadLeftPressed) action = padLeft;
				else if (PadRightPressed) action = padRight;
				else if (PadBottomPressed) action = padBottom;
				else if (PadCentrePressed) action = padCentre;
			} else
			{
				action = padCentreOculus;
			}
			if (action >= VRActions.Length)
			{
				Debug.LogWarning("Pad key index (" + action + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[action]);
		}

		protected void TrackpadUp()
		{
			if (hmdType == HMDType.VIVE)
			{
				for(int i=0; i<VRActions.Length; i++)
				{
					if (padLeft == i || padTop == i || padRight == i || padBottom == i || padCentre == i)
						SendMessageToInteractor(VRActions[i]+"Released");
				}
			} else
			{
				SendMessageToInteractor(VRActions[padCentreOculus]+"Released");
			}
		}

		protected void TrackpadTouch()
		{
			int touchKey = this.padTouch;
			if (hmdType == HMDType.OCULUS) touchKey = this.padTouchOculus;
			if (touchKey >= VRActions.Length)
			{
				Debug.LogWarning("Touch key index (" + touchKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[touchKey]);
		}

		protected void TrackpadUnTouch()
		{
			int touchKey = this.padTouch;
			if (hmdType == HMDType.OCULUS) touchKey = this.padTouchOculus;
			if (touchKey >= VRActions.Length)
			{
				Debug.LogWarning("Touch key index (" + touchKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[touchKey]+"Released");
		}

		protected void Gripped()
		{
			int gripKey = this.gripKey;
			if (hmdType == HMDType.OCULUS) gripKey = this.gripKeyOculus;
			if (gripKey >= VRActions.Length)
			{
				Debug.LogWarning("Gripped key index (" + gripKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[gripKey]);
		}

		protected void UnGripped()
		{
			int gripKey = this.gripKey;
			if (hmdType == HMDType.OCULUS) gripKey = this.gripKeyOculus;
			if (gripKey >= VRActions.Length)
			{
				Debug.LogWarning("Gripped key index (" + gripKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[gripKey]+"Released");
		}

		protected void MenuClicked()
		{
			int menuKey = this.menuKey;
			if (hmdType == HMDType.OCULUS) menuKey = this.menuKeyOculus;
			if (menuKey >= VRActions.Length)
			{
				Debug.LogWarning("Menu key index (" + menuKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[menuKey]);
		}

		protected void MenuReleased()
		{
			int menuKey = this.menuKey;
			if (hmdType == HMDType.OCULUS) menuKey = this.menuKeyOculus;
			if (menuKey >= VRActions.Length)
			{
				Debug.LogWarning("Menu key index (" + menuKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[menuKey]+"Released");
		}

		protected void AXClicked()
		{
			int aButtonKey = this.AXKey;
			if (hmdType == HMDType.OCULUS) aButtonKey = this.AXKeyOculus;
			if (aButtonKey >= VRActions.Length)
			{
				Debug.LogWarning("A Button key index (" + aButtonKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[aButtonKey]);
		}

		protected void AXReleased()
		{
			int aButtonKey = this.AXKey;
			if (hmdType == HMDType.OCULUS) aButtonKey = this.AXKeyOculus;
			if (aButtonKey >= VRActions.Length)
			{
				Debug.LogWarning("A Button key index (" + aButtonKey + ") out of range (" + VRActions.Length+")");
				return;
			}
			SendMessageToInteractor(VRActions[aButtonKey]+"Released");
		}
	}

}
