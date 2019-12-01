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

        protected enum Keys
        {
            TRIGGER,
            PAD_TOP,
            PAD_LEFT,
            PAD_RIGHT,
            PAD_BOTTOM,
            PAD_CENTRE,
            PAD_TOUCH,
            GRIP,
            MENU,
            AX,
            TRIGGER_OCULUS,
            PAD_TOP_OCULUS,
            PAD_LEFT_OCULUS,
            PAD_RIGHT_OCULUS,
            PAD_BOTTOM_OCULUS,
            PAD_CENTRE_OCULUS,
            PAD_TOUCH_OCULUS,
            GRIP_OCULUS,
            MENU_OCULUS,
            AX_OCULUS
        }

		public string[] VRActions;

		//Used in the editor
		public bool mirrorControls = true;
		// Will display oculus buttons when false
		public bool displayViveButtons;

		public int triggerKey;
        public List<int> triggerKeys = new List<int>();
        public int padTop;
        public List<int> padTops = new List<int>();
        public int padLeft;
        public List<int> padLefts = new List<int>();
        public int padRight;
        public List<int> padRights = new List<int>();
        public int padBottom;
        public List<int> padBottoms = new List<int>();
        public int padCentre;
        public List<int> padCentres = new List<int>();
        public int padTouch;
        public List<int> padTouchs = new List<int>();
        public int gripKey;
        public List<int> gripKeys = new List<int>();
        public int menuKey;
        public List<int> menuKeys = new List<int>();
        public int AXKey;
        public List<int> AXKeys = new List<int>();

        //Oculus alternative buttons
        public int triggerKeyOculus;
        public List<int> triggerKeysOculus = new List<int>();
        public int padTopOculus;
        public List<int> padTopsOculus = new List<int>();
        public int padLeftOculus;
        public List<int> padLeftsOculus = new List<int>();
        public int padRightOculus;
        public List<int> padRightsOculus = new List<int>();
        public int padBottomOculus;
        public List<int> padBottomsOculus = new List<int>();
        public int padCentreOculus;
        public List<int> padCentresOculus = new List<int>();
        public int padTouchOculus;
        public List<int> padTouchsOculus = new List<int>();
        public int gripKeyOculus;
        public List<int> gripKeysOculus = new List<int>();
        public int menuKeyOculus;
        public List<int> menuKeysOculus = new List<int>();
        public int AXKeyOculus;
        public List<int> AXKeysOculus = new List<int>();

#if Int_SteamVR2

        public SteamVR_Input_Sources handType;
		public List<SteamVR_Action_Boolean> booleanActions = new List<SteamVR_Action_Boolean>();
		public SteamVR_Action_Single triggerPressure = SteamVR_Input.GetAction<SteamVR_Action_Single>("TriggerPressure");
		public SteamVR_Action_Vector2 touchPosition = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("TouchPosition");
		public SteamVR_Action_Boolean padTouched = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadTouched");
		public SteamVR_Action_Boolean padPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadPressed");

		#endif

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
			#if !(Int_SteamVR && !Int_SteamVR2)
			if (!isSteamVR())
			{
			#endif
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
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_LEFT_OCULUS, false));
				    } else if (!PadLeftPressed && _stickLeftDown)
					    _stickLeftDown = false;

				    if (PadRightPressed && !_stickRightDown)
				    {
					    _stickRightDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_RIGHT_OCULUS, false));
				    } else if (!PadRightPressed && _stickRightDown)
					    _stickRightDown = false;

				    if (PadBottomPressed && !_stickBottomDown)
				    {
					    _stickBottomDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_BOTTOM_OCULUS, false));
				    } else if (!PadBottomPressed && _stickBottomDown)
					    _stickBottomDown = false;

				    if (PadTopPressed && !_stickTopDown)
				    {
					    _stickTopDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_TOP_OCULUS, false));
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
			
			#if !(Int_SteamVR && !Int_SteamVR2)
			}
			#endif
			#if Int_SteamVR2

			foreach(SteamVR_Action_Boolean boolAction in booleanActions)
			{
				if (boolAction == null)
				{
					Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
					continue;
				}
				if (boolAction.GetStateDown(handType))
				{
					SendMessageToInteractor(boolAction.GetShortName());
				}
				if (boolAction.GetStateUp(handType))
				{
					SendMessageToInteractor(boolAction.GetShortName()+"Released");
				}
			}

			#endif
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
			#if Int_SteamVR
			if (GetComponent<SteamVR_TrackedObject>() != null || GetComponentInParent<SteamVR_PlayArea>() != null)
				return true;
			else
			{
				#if Int_SteamVR2
				if (GetComponent<SteamVR_Behaviour_Pose>() != null) return true;
				#endif
				return false;
			}
			#elif Int_Oculus
			return false;
			#else
			throw new System.Exception("Requires SteamVR or Oculus Integration asset. If one is already imported try re-importing, in the project window right click->Re-Import All.");
			#endif
		}
		public string[] getVRActions{get { return VRActions; } set { VRActions = value; }}
		
		virtual public HMDType hmdType
		{
			get
			{
			#if Int_SteamVR
			if ((GetComponent<SteamVR_TrackedObject>() != null || GetComponentInParent<SteamVR_PlayArea>() != null) || (SteamVR.active && SteamVR.instance != null && SteamVR.instance.hmd_TrackingSystemName != "oculus"))
				return HMDType.VIVE; 
			else
				return HMDType.OCULUS;
			#elif Int_Oculus
			return HMDType.OCULUS;
			#else
			throw new System.Exception("Requires SteamVR or Oculus Integration asset. If one is already imported try re-importing, in the project window right click->Re-Import All.");
			#endif
			}
		}
		virtual public bool LeftHand
		{
			get 
			{
				#if Int_SteamVR
				if (isSteamVR())
				{
					#if !Int_SteamVR2
					SteamVR_ControllerManager controllerManager = null;
					if (transform.parent != null) controllerManager = transform.parent.GetComponent<SteamVR_ControllerManager>();
					else controllerManager = FindObjectOfType<SteamVR_ControllerManager>();
					if (controllerManager != null) return gameObject == controllerManager.left;
					else
					{
						Debug.LogError("Can't find SteamVR_ControllerManager in scene");
					}
					#else
					if (name.ToUpper().Contains("LEFT"))
						handType = SteamVR_Input_Sources.LeftHand;
					else handType = SteamVR_Input_Sources.RightHand;
					return handType == SteamVR_Input_Sources.LeftHand;
					#endif
				}
				#endif
				
				#if Int_Oculus
				if (!isSteamVR())
				{
					OvrAvatar avatar = GetComponentInParent<OvrAvatar>();
					if (avatar == null)
					{
						if (name.ToUpper().Contains("LEFT"))
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
					return controllerHand == OVRInput.Controller.LTouch;
				}
				#endif
				return false;
			}
		}

		public bool ActionPressed(string action)
		{
		#if Int_SteamVR && !Int_SteamVR2
			if (VRActions != null)
		#else
			if (VRActions != null && !isSteamVR())
		#endif
			{
				for(int i=0; i<VRActions.Length; i++)
				{
					if (action == GetAction(i))
					{
						return ActionPressed(i);
					}
				}
			}
			#if Int_SteamVR2
			foreach (SteamVR_Action_Boolean booleanAction in booleanActions)
			{
				if (booleanAction == null)
				{
					Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
					continue;
				}
				if (booleanAction.GetShortName() == action)
				{
					return booleanAction.GetState(handType);
				}
			}
			#endif
			return false;
		}

		public bool ActionPressed(int action)
		{
			if (hmdType == HMDType.VIVE)
			{
				if ((triggerKey == action || triggerKeys.Contains(action)) && TriggerPressed)
					return true;
				if ((padTop == action || padTops.Contains(action))  && PadTopPressed)
					return true;
				if ((padLeft == action || padLefts.Contains(action))  && PadLeftPressed)
					return true;
				if ((padRight == action || padRights.Contains(action))  && PadRightPressed)
					return true;
				if ((padBottom == action || padBottoms.Contains(action))  && PadBottomPressed)
					return true;
				if ((padCentre == action || padCentres.Contains(action))  && PadCentrePressed)
					return true;
				if ((padTouch == action || padTouchs.Contains(action))  && PadTouched)
					return true;
				if ((menuKey == action || menuKeys.Contains(action))  && MenuPressed)
					return true;
				if ((gripKey == action || gripKeys.Contains(action))  && GripPressed)
					return true;
				if ((AXKey == action || AXKeys.Contains(action))  && AXPressed)
					return true;
			} else
			{
				if ((triggerKeyOculus == action || triggerKeysOculus.Contains(action)) && TriggerPressed)
					return true;
				if ((padTopOculus == action || padTopsOculus.Contains(action)) && PadTopPressed)
					return true;
				if ((padLeftOculus == action || padLeftsOculus.Contains(action)) && PadLeftPressed)
					return true;
				if ((padRightOculus == action || padRightsOculus.Contains(action)) && PadRightPressed)
					return true;
				if ((padBottomOculus == action || padBottomsOculus.Contains(action)) && PadBottomPressed)
					return true;
				if ((padCentreOculus == action || padCentresOculus.Contains(action)) && PadCentrePressed)
					return true;
				if ((padTouchOculus == action || padTouchsOculus.Contains(action)) && PadTouched)
					return true;
				if ((menuKeyOculus == action || menuKeysOculus.Contains(action)) && MenuPressed)
					return true;
				if ((gripKeyOculus == action || gripKeysOculus.Contains(action)) && GripPressed)
					return true;
				if ((AXKeyOculus == action || AXKeysOculus.Contains(action)) && AXPressed)
					return true;
			}
			return false;
		}

		virtual public bool TriggerPressed
		{
			get
			{
				return TriggerPressure > 0.5f;
			}
		}
		virtual public float TriggerPressure
		{
			get
			{
			#if Int_SteamVR
			if (isSteamVR())
			{
			#if !Int_SteamVR2

				var device = SteamVR_Controller.Input((int)controller.controllerIndex);
				return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
			#else
				if (triggerPressure != null) return triggerPressure.GetAxis(handType);
				else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
			#endif
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					if (PadPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						Vector2 axis = PadPosition;
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					if (PadPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						Vector2 axis = PadPosition;
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					if (PadPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						Vector2 axis = PadPosition;
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					if (PadPressed || (hmdType == HMDType.OCULUS && PadTouched))
					{
						Vector2 axis = PadPosition;
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					if (PadPressed)
					{
						Vector2 axis = PadPosition;
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					#if !Int_SteamVR2
					return controller.padTouched;
					#else
					if (padTouched != null) return padTouched.GetState(handType);
					else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
					#endif
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					#if !Int_SteamVR2
					return controller.padPressed;
					#else
					if (padPressed != null) return padPressed.GetState(handType);
					else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
					#endif
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
				#if Int_SteamVR
				if (isSteamVR())
				{
					#if !Int_SteamVR2
					var device = SteamVR_Controller.Input((int)controller.controllerIndex);
					return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
					#else
					if (touchPosition != null) return touchPosition.GetAxis(handType);
					else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
					#endif
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

        virtual public void SendMessageToInteractor(List<string> actions)
        {
            foreach (string action in actions) SendMessageToInteractor(action);
        }

        virtual public void SendMessageToInteractor(string action)
        {
            SendMessage("InputReceived", action, SendMessageOptions.DontRequireReceiver);
		}

        virtual protected List<string> GetAllActionsForButton(Keys key, bool released)
        {
            List<string> returnList = new List<string>();
            switch(key)
            {
                case Keys.TRIGGER:
                    returnList.Add(GetAction(triggerKey) + (released ? "Released" : ""));
                    foreach (int index in triggerKeys) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_TOP:
                    returnList.Add(GetAction(padTop) + (released ? "Released" : ""));
                    foreach (int index in padTops) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_LEFT:
                    returnList.Add(GetAction(padLeft) + (released ? "Released" : ""));
                    foreach (int index in padLefts) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_RIGHT:
                    returnList.Add(GetAction(padRight) + (released ? "Released" : ""));
                    foreach (int index in padRights) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_BOTTOM:
                    returnList.Add(GetAction(padBottom) + (released ? "Released" : ""));
                    foreach (int index in padBottoms) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_CENTRE:
                    returnList.Add(GetAction(padCentre) + (released ? "Released" : ""));
                    foreach (int index in padCentres) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_TOUCH:
                    returnList.Add(GetAction(padTouch) + (released ? "Released" : ""));
                    foreach (int index in padTouchs) returnList.Add(GetAction(index));
                    break;
                case Keys.GRIP:
                    returnList.Add(GetAction(gripKey) + (released ? "Released" : ""));
                    foreach (int index in gripKeys) returnList.Add(GetAction(index));
                    break;
                case Keys.MENU:
                    returnList.Add(GetAction(menuKey) + (released ? "Released" : ""));
                    foreach (int index in menuKeys) returnList.Add(GetAction(index));
                    break;
                case Keys.AX:
                    returnList.Add(GetAction(AXKey) + (released ? "Released" : ""));
                    foreach (int index in AXKeys) returnList.Add(GetAction(index));
                    break;
                case Keys.TRIGGER_OCULUS:
                    returnList.Add(GetAction(triggerKeyOculus) + (released ? "Released" : ""));
                    foreach (int index in triggerKeysOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_TOP_OCULUS:
                    returnList.Add(GetAction(padTopOculus) + (released ? "Released" : ""));
                    foreach (int index in padTopsOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_LEFT_OCULUS:
                    returnList.Add(GetAction(padLeftOculus) + (released ? "Released" : ""));
                    foreach (int index in padLeftsOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_RIGHT_OCULUS:
                    returnList.Add(GetAction(padRightOculus) + (released ? "Released" : ""));
                    foreach (int index in padRightsOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_BOTTOM_OCULUS:
                    returnList.Add(GetAction(padBottomOculus) + (released ? "Released" : ""));
                    foreach (int index in padBottomsOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_CENTRE_OCULUS:
                    returnList.Add(GetAction(padCentreOculus) + (released ? "Released" : ""));
                    foreach (int index in padCentresOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.PAD_TOUCH_OCULUS:
                    returnList.Add(GetAction(padTouchOculus) + (released ? "Released" : ""));
                    foreach (int index in padTouchsOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.GRIP_OCULUS:
                    returnList.Add(GetAction(gripKeyOculus) + (released ? "Released" : ""));
                    foreach (int index in gripKeysOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.MENU_OCULUS:
                    returnList.Add(GetAction(menuKeyOculus) + (released ? "Released" : ""));
                    foreach (int index in menuKeysOculus) returnList.Add(GetAction(index));
                    break;
                case Keys.AX_OCULUS:
                    returnList.Add(GetAction(AXKeyOculus) + (released ? "Released" : ""));
                    foreach (int index in AXKeysOculus) returnList.Add(GetAction(index));
                    break;
            }
            return returnList;
        }

        private string GetAction(int index)
        {
            if (index >= VRActions.Length)
            {
                Debug.LogError("index (" + index + ") out of range (" + VRActions.Length + "). " +
                    "A button is assigned an index that is bigger than the actions. Check VRInput component actions. Controller Name: " + name, gameObject);
                return "";
            }
            return VRActions[index];
        }

		protected void TriggerClicked()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.TRIGGER_OCULUS : Keys.TRIGGER, false));
		}

		protected void TriggerReleased()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.TRIGGER_OCULUS : Keys.TRIGGER, true));
		}

		protected void TrackpadDown()
		{
            Keys key = Keys.TRIGGER;
			if (hmdType == HMDType.VIVE)
			{
                if (PadTopPressed) key = Keys.PAD_TOP;
                else if (PadLeftPressed) key = Keys.PAD_LEFT;
                else if (PadRightPressed) key = Keys.PAD_RIGHT;
                else if (PadBottomPressed) key = Keys.PAD_BOTTOM;
                else if (PadCentrePressed) key = Keys.PAD_CENTRE;
            } else
			{
                key = Keys.PAD_CENTRE_OCULUS;
			}
			
            SendMessageToInteractor(GetAllActionsForButton(key, false));
		}

		protected void TrackpadUp()
		{
			if (hmdType == HMDType.VIVE)
			{
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_TOP, true));
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_LEFT, true));
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_RIGHT, true));
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_BOTTOM, true));
                //for(int i=0; i<VRActions.Length; i++)
                //{
                //	if (padLeft == i || padTop == i || padRight == i || padBottom == i || padCentre == i)
                //		SendMessageToInteractor(VRActions[i]+"Released");
                //}
            } else
			{
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_CENTRE_OCULUS, true));
                //SendMessageToInteractor(VRActions[padCentreOculus]+"Released");
			}
		}

		protected void TrackpadTouch()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.PAD_TOUCH_OCULUS : Keys.PAD_TOUCH, false));
		}

		protected void TrackpadUnTouch()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.PAD_TOUCH_OCULUS : Keys.PAD_TOUCH, true));
		}

		protected void Gripped()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.GRIP_OCULUS : Keys.GRIP, false));
		}

		protected void UnGripped()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.GRIP_OCULUS : Keys.GRIP, true));
		}

		protected void MenuClicked()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.MENU_OCULUS : Keys.MENU, false));
		}

		protected void MenuReleased()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.MENU_OCULUS : Keys.MENU, true));
		}

		protected void AXClicked()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.AX_OCULUS : Keys.AX, false));
		}

		protected void AXReleased()
		{
            SendMessageToInteractor(GetAllActionsForButton(hmdType == HMDType.OCULUS ? Keys.AX_OCULUS : Keys.AX, true));
		}
	}

}
