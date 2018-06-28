using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VRInteraction
{
	public class VRInteractorRemote : VRInteractor 
	{
		//Sync vars
		public Vector3 _velocity;
		public Vector3 _angularVelocity;

		override public Transform GetVRRigRoot 
		{
			get 
			{
				if (_vrRigRoot == null) _vrRigRoot = GetComponentInParent<NetworkIdentity>().transform;
				return transform.root;
			}
		}

		override public Vector3 Velocity
		{
			get
			{
				return _velocity;
			}
		}

		override public Vector3 AngularVelocity
		{
			get 
			{
				return _angularVelocity;
			}
		}

		override public void TriggerHapticPulse(int frames)
		{
			//This is for someone else
		}
	}
}