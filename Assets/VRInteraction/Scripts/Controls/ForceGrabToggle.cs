using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class ForceGrabToggle : MonoBehaviour 
	{
		public string actionName = "PICKUP_DROP";
		public float defaultForceDistance = 5f;

		private VRInteractor vrInteractor;
		private bool forceGrabOn;

		void Update()
		{
			if (vrInteractor != null && vrInteractor.forceGrabDistance == (forceGrabOn ? defaultForceDistance : 0f)) return;
			if (vrInteractor == null) vrInteractor = GetComponent<VRInteractor>();
			if (vrInteractor == null) return;
			vrInteractor.forceGrabDistance = forceGrabOn ? defaultForceDistance : 0f;
		}

		public void InputReceived(string method)
		{
			if (vrInteractor == null)
			{
				vrInteractor = GetComponent<VRInteractor>();
				if (vrInteractor == null) 
				{
					Debug.LogError("This script should be on the same object as a VRInteractor");
					return;
				}
			}

			if (method == actionName)
			{
				forceGrabOn = true;
				vrInteractor.forceGrabDistance = defaultForceDistance;
			}
			else if (method == actionName+"Released")
			{
				forceGrabOn = false;
				vrInteractor.forceGrabDistance = 0f;
			}
		}
	}
}
