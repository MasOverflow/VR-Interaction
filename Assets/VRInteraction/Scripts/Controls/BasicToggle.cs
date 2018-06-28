//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Basic toggle. This script should be attached to the same object as an interacable item.
// If held it will enable and disable the target object when ACTION is pressed.
// This script should be above the VRInteracableItem script so the ACTION is called after
// the item ACTION, this will stop the toggle being trigger when picking up an item with
// the ACTION key.
//
//===================Contact Email: Sam@MassGames.co.uk===========================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class BasicToggle : MonoBehaviour {

		public GameObject target;
		public bool startEnabled;

		private bool _enabled;
		private VRInteractableItem item;

		void Start()
		{
			if (target == null) 
			{
				Debug.LogError("No Target specified", gameObject);
				return;
			}
			item = GetComponent<VRInteractableItem>();
			if (item == null)
			{
				Debug.LogError("This script requires an VRInteracableItem script on the same object", gameObject);
				return;
			}
			target.gameObject.SetActive(startEnabled);
			_enabled = startEnabled;
		}

		void ACTION(VRInteractor hand)
		{
			if (target == null || item == null || hand.heldItem != item) return;
			_enabled = !_enabled;
			target.gameObject.SetActive(_enabled);
		}
	}
}