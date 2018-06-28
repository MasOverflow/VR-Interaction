//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// This script takes input from a connected input script from the server
//
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VRInteraction
{
	public class VRInputRemote : VRInput
	{
		//Const vars to sync
		public bool _isSteamVR;
		public HMDType _hmdType;
		public bool _leftHand;

		//Vars to sync
		public bool _triggerPressed;
		public float _triggerPressure;
		public bool _padTopPressed;
		public bool _padLeftPressed;
		public bool _padRightPressed;
		public bool _padBottomPressed;
		public bool _padCentrePressed;
		public bool _padTouched;
		public bool _padPressed;
		public Vector2 _padPosition;
		public bool _gripPressed;
		public bool _menuPressed;
		public bool _axPressed;

		override public bool isSteamVR(){return _isSteamVR;}
		override public HMDType hmdType{get {return _hmdType;}}
		override public bool LeftHand{get {	return _leftHand; }}
		override public bool TriggerPressed{get { return _triggerPressed; }}
		override public float TriggerPressure{get{return _triggerPressure;}}
		override public bool PadTopPressed{get{ return _padTopPressed; }}
		override public bool PadLeftPressed{get{return _padLeftPressed;}}
		override public bool PadRightPressed{get{return _padRightPressed;}}
		override public bool PadBottomPressed{get{return _padBottomPressed;}}
		override public bool PadCentrePressed{get{ return _padCentrePressed;}}
		override public bool PadTouched{get { return _padTouched; }}
		override public bool PadPressed{get { return _padPressed; }}
		override public Vector2 PadPosition{get	{return _padPosition;}}
		override public bool GripPressed{get { return _gripPressed; }}
		override public bool MenuPressed{get { return _menuPressed; }}
		override public bool AXPressed{	get	{ return _axPressed;}}
	}
}