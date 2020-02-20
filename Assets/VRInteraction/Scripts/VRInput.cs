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
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#endif
#if Int_SteamVR
using Valve.VR;
#endif

namespace VRInteraction
{
    public class VRInput : MonoBehaviour
    {
        public enum JoystickType
        {
            Trackpad,
            Thumbstick
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
            PRIMARY,
            SECONDARY
        }

        public bool trackPositionNatively;
        public bool trackInputNatively;
        public bool LeftHand;

        public string[] VRActions;

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
        public int primaryKey;
        public List<int> primaryKeys = new List<int>();
        public int secondaryKey;
        public List<int> secondaryKeys = new List<int>();

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
        private bool _Primary_PressedFlag = false;
        private bool _Secondary_PressedFlag = false;

        private bool _stickLeftDown;
        private bool _stickTopDown;
        private bool _stickBottomDown;
        private bool _stickRightDown;

        private InputDevice _currentInputDevice;

        virtual protected void Start()
        {
        }
        
        virtual protected void Update()
        {
#if UNITY_2017_2_OR_NEWER
            if (trackPositionNatively)
            {
                XRNodeState nodeState = GetXRNodeState();
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;
                nodeState.TryGetPosition(out pos);
                nodeState.TryGetRotation(out rot);
                transform.localPosition = pos;
                transform.localRotation = rot;
            }
#endif

            if (trackInputNatively)
            {
                bool trigger = TriggerPressed;
                if (trigger && !_triggerPressedFlag)
                {
                    _triggerPressedFlag = true;
                    TriggerClicked();
                }
                else if (!trigger && _triggerPressedFlag)
                {
                    _triggerPressedFlag = false;
                    TriggerReleased();
                }

                bool thumbstick = PadPressed;
                if (thumbstick && !_padPressedFlag)
                {
                    _padPressedFlag = true;
                    TrackpadDown();
                }
                else if (!thumbstick && _padPressedFlag)
                {
                    _padPressedFlag = false;
                    TrackpadUp();
                }

                bool thumbstickTouch = PadTouched;
                if (thumbstickTouch && !_padTouchedFlag)
                {
                    _padTouchedFlag = true;
                    TrackpadTouch();
                }
                else if (!thumbstickTouch && _padTouchedFlag)
                {
                    _padTouchedFlag = false;
                    _stickLeftDown = false;
                    _stickTopDown = false;
                    _stickBottomDown = false;
                    _stickRightDown = false;
                    TrackpadUnTouch();
                }
                if (joystickType == JoystickType.Thumbstick && _padTouchedFlag)
                {
                    if (PadLeftPressed && !_stickLeftDown)
                    {
                        _stickLeftDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_LEFT, false));
                    }
                    else if (!PadLeftPressed && _stickLeftDown)
                        _stickLeftDown = false;

                    if (PadRightPressed && !_stickRightDown)
                    {
                        _stickRightDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_RIGHT, false));
                    }
                    else if (!PadRightPressed && _stickRightDown)
                        _stickRightDown = false;

                    if (PadBottomPressed && !_stickBottomDown)
                    {
                        _stickBottomDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_BOTTOM, false));
                    }
                    else if (!PadBottomPressed && _stickBottomDown)
                        _stickBottomDown = false;

                    if (PadTopPressed && !_stickTopDown)
                    {
                        _stickTopDown = true;
                        SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_TOP, false));
                    }
                    else if (!PadTopPressed && _stickTopDown)
                        _stickTopDown = false;
                }

                bool grip = GripPressed;
                if (grip && !_grippedFlag)
                {
                    _grippedFlag = true;
                    Gripped();
                }
                else if (!grip && _grippedFlag)
                {
                    _grippedFlag = false;
                    UnGripped();
                }

                bool menu = MenuPressed;
                if (menu && !_menuPressedFlag)
                {
                    _menuPressedFlag = true;
                    MenuClicked();
                }
                else if (!menu && _menuPressedFlag)
                {
                    _menuPressedFlag = false;
                    MenuReleased();
                }

                bool primary = PrimaryButton;
                if (primary && !_Primary_PressedFlag)
                {
                    _Primary_PressedFlag = true;
                    PrimaryClicked();
                }
                else if (!primary && _Primary_PressedFlag)
                {
                    _Primary_PressedFlag = false;
                    PrimaryReleased();
                }

                bool secondary = SecondaryButton;
                if (secondary && !_Secondary_PressedFlag)
                {
                    _Secondary_PressedFlag = true;
                    SecondaryClicked();
                }
                else if (!secondary && _Secondary_PressedFlag)
                {
                    _Secondary_PressedFlag = false;
                    SecondaryReleased();
                }

            }
            else
            {
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
        }

        virtual public bool isSteamVR()
        {
            return XRSettings.loadedDeviceName == "OpenVR";
        }
        public string[] getVRActions { get { return VRActions; } set { VRActions = value; } }

        virtual public JoystickType joystickType
        {
            get
            {
                //Debug.Log(XRDevice.model);
                if (XRDevice.model == "Vive MV") return JoystickType.Trackpad;
                else return JoystickType.Thumbstick;
            }
        }

        public bool ActionPressed(string action)
        {
            if (trackInputNatively)
            {
                if (VRActions != null)
                {
                    for (int i = 0; i < VRActions.Length; i++)
                    {
                        if (action == GetAction(i))
                        {
                            return ActionPressed(i);
                        }
                    }
                }
            } else
            {
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
            }
            return false;
        }

        public bool ActionPressed(int action)
        {
            if ((triggerKey == action || triggerKeys.Contains(action)) && TriggerPressed)
                return true;
            if ((padTop == action || padTops.Contains(action)) && PadTopPressed)
                return true;
            if ((padLeft == action || padLefts.Contains(action)) && PadLeftPressed)
                return true;
            if ((padRight == action || padRights.Contains(action)) && PadRightPressed)
                return true;
            if ((padBottom == action || padBottoms.Contains(action)) && PadBottomPressed)
                return true;
            if ((padCentre == action || padCentres.Contains(action)) && PadCentrePressed)
                return true;
            if ((padTouch == action || padTouchs.Contains(action)) && PadTouched)
                return true;
            if ((menuKey == action || menuKeys.Contains(action)) && MenuPressed)
                return true;
            if ((gripKey == action || gripKeys.Contains(action)) && GripPressed)
                return true;
            if ((primaryKey == action || primaryKeys.Contains(action)) && PrimaryButton)
                return true;
            if ((secondaryKey == action || secondaryKeys.Contains(action)) && SecondaryButton)
                return true;
            return false;
        }

        public XRNodeState GetXRNodeState()
        {
            List<XRNodeState> nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);
            XRNodeState nodeState = new XRNodeState();
            foreach (XRNodeState posNodeState in nodeStates)
            {
                if (LeftHand && posNodeState.nodeType == XRNode.LeftHand) nodeState = posNodeState;
                else if (!LeftHand && posNodeState.nodeType == XRNode.RightHand) nodeState = posNodeState;
            }
            return nodeState;
        }

        public InputDevice currentInputDevice
        {
            get
            {
                if (!_currentInputDevice.isValid) RefreshInputDevice();
                return _currentInputDevice;
            }
        }

        private void RefreshInputDevice()
        {
            if (!trackInputNatively) return;
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);
            foreach (InputDevice device in devices)
            {
                if (LeftHand && device.role == InputDeviceRole.LeftHanded) _currentInputDevice = device;
                else if (!LeftHand && device.role == InputDeviceRole.RightHanded) _currentInputDevice = device;
            }
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
                if (trackInputNatively)
                { 
                    float triggerPressure = 0f;
                    currentInputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerPressure);
                    return triggerPressure;
                } else
                {
#if Int_SteamVR2
                    if (triggerPressure != null) return triggerPressure.GetState(handType);
				    else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
#endif
                }
                return 0f;
            }
        }

        virtual public bool PadTopPressed
        {
            get
            {
                if ((joystickType == JoystickType.Trackpad && PadPressed) || (joystickType == JoystickType.Thumbstick && PadTouched))
                {
                    Vector2 axis = PadPosition;
                    if (axis.y > (joystickType == JoystickType.Trackpad ? 0.4f : 0.8f) &&
                        axis.x < axis.y &&
                        axis.x > -axis.y)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        virtual public bool PadLeftPressed
        {
            get
            {
                if ((joystickType == JoystickType.Trackpad && PadPressed) || (joystickType == JoystickType.Thumbstick && PadTouched))
                {
                    Vector2 axis = PadPosition;
                    if (axis.x < (joystickType == JoystickType.Trackpad ? -0.4f : -0.5f) &&
                            axis.y > axis.x &&
                            axis.y < -axis.x)
                        return true;
                }
                return false;
            }
        }
        virtual public bool PadRightPressed
        {
            get
            {
                if ((joystickType == JoystickType.Trackpad && PadPressed) || (joystickType == JoystickType.Thumbstick && PadTouched))
                {
                    Vector2 axis = PadPosition;
                    if (axis.x > (joystickType == JoystickType.Trackpad ? 0.4f : 0.5f) &&
                            axis.y < axis.x &&
                            axis.y > -axis.x)
                        return true;
                }
                return false;
            }
        }
        virtual public bool PadBottomPressed
        {
            get
            {
                if ((joystickType == JoystickType.Trackpad && PadPressed) || (joystickType == JoystickType.Thumbstick && PadTouched))
                {
                    Vector2 axis = PadPosition;
                    if ((axis.y < (joystickType == JoystickType.Trackpad ? -0.4f : -0.8f) &&
                            axis.x > axis.y &&
                            axis.x < -axis.y))
                        return true;
                }
                return false;
            }
        }
        virtual public bool PadCentrePressed
        {
            get
            {
                if ((joystickType == JoystickType.Trackpad && PadPressed) || (joystickType == JoystickType.Thumbstick && PadTouched))
                {
                    Vector2 axis = PadPosition;
                    if (axis.y >= -0.4f && axis.y <= 0.4f && axis.x >= -0.4f && axis.x <= 0.4f)
                        return true;
                }
                return false;
            }
        }
        virtual public bool PadTouched
        {
            get
            {
                if (trackInputNatively)
                {
                    bool touched = false;
                    currentInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out touched);
                    return touched;
                } else
                {
#if Int_SteamVR2
                    if (padTouched != null) return padTouched.GetState(handType);
					else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
#endif
                }
                return false;
            }
        }
        virtual public bool PadPressed
        {
            get
            {
                if (trackInputNatively)
                {
                    bool pressed = false;
                    currentInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out pressed);
                    return pressed;
            } else
            {
#if Int_SteamVR2
                if (padPressed != null) return padPressed.GetState(handType);
				else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
#endif
                }
                return false;
            }
        }
        virtual public Vector2 PadPosition
        {
            get
            {
                if (trackInputNatively)
                {
                    Vector2 axis = Vector2.zero;
                    currentInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
                    return axis;
                } else
                {
#if Int_SteamVR2
                    if (touchPosition != null) return touchPosition.GetAxis(handType);
					else Debug.LogError("SteamVR Inputs have not been setup. Refer to the SteamVR 2.0 section of the Setup Guide. Found in Assets/VRInteraction/Docs.");
#endif
                }
                return Vector2.zero;
            }
        }
        virtual public bool GripPressed
        {
            get
            {
                bool pressed = false;
                currentInputDevice.TryGetFeatureValue(CommonUsages.gripButton, out pressed);
                return pressed;
            }
        }
        virtual public bool MenuPressed
        {
            get
            {
                bool pressed = false;
                currentInputDevice.TryGetFeatureValue(CommonUsages.menuButton, out pressed);
                return pressed;
            }
        }
        virtual public bool PrimaryButton
        {
            get
            {
                bool pressed = false;
                currentInputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);
                return pressed;
            }
        }
        virtual public bool SecondaryButton
        {
            get
            {
                bool pressed = false;
                currentInputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out pressed);
                return pressed;
            }
        }

        public bool isTriggerPressed { get { return _triggerPressedFlag; } }
        public bool isPadPressed { get { return _padPressedFlag; } }
        public bool isPadTouched { get { return _padTouchedFlag; } }
        public bool isGripped { get { return _grippedFlag; } }
        public bool isBY_Pressed { get { return _menuPressedFlag; } }
        public bool isPrimary_Pressed { get { return _Primary_PressedFlag; } }
        public bool isSecondary_Pressed { get { return _Secondary_PressedFlag; } }

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
            switch (key)
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
                case Keys.PRIMARY:
                    returnList.Add(GetAction(primaryKey) + (released ? "Released" : ""));
                    foreach (int index in primaryKeys) returnList.Add(GetAction(index));
                    break;
                case Keys.SECONDARY:
                    returnList.Add(GetAction(secondaryKey) + (released ? "Released" : ""));
                    foreach (int index in secondaryKeys) returnList.Add(GetAction(index));
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
            SendMessageToInteractor(GetAllActionsForButton(Keys.TRIGGER, false));
        }

        protected void TriggerReleased()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.TRIGGER, true));
        }

        protected void TrackpadDown()
        {
            Keys key = Keys.TRIGGER;
            if (joystickType == JoystickType.Trackpad)
            {
                if (PadTopPressed) key = Keys.PAD_TOP;
                else if (PadLeftPressed) key = Keys.PAD_LEFT;
                else if (PadRightPressed) key = Keys.PAD_RIGHT;
                else if (PadBottomPressed) key = Keys.PAD_BOTTOM;
                else if (PadCentrePressed) key = Keys.PAD_CENTRE;
            }
            else
            {
                key = Keys.PAD_CENTRE;
            }
            SendMessageToInteractor(GetAllActionsForButton(key, false));
        }

        protected void TrackpadUp()
        {
            if (joystickType == JoystickType.Trackpad)
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
            }
            else
            {
                SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_CENTRE, true));
                //SendMessageToInteractor(VRActions[padCentreOculus]+"Released");
            }
        }

        protected void TrackpadTouch()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_TOUCH, false));
        }

        protected void TrackpadUnTouch()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.PAD_TOUCH, true));
        }

        protected void Gripped()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.GRIP, false));
        }

        protected void UnGripped()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.GRIP, true));
        }

        protected void MenuClicked()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.MENU, false));
        }

        protected void MenuReleased()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.MENU, true));
        }

        protected void PrimaryClicked()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.PRIMARY, false));
        }

        protected void PrimaryReleased()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.PRIMARY, true));
        }

        protected void SecondaryClicked()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.SECONDARY, false));
        }

        protected void SecondaryReleased()
        {
            SendMessageToInteractor(GetAllActionsForButton(Keys.SECONDARY, true));
        }
    }

}
