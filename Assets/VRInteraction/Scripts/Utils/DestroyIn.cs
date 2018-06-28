//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Destroys or disables the attached object in the given time after being enabled
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using System.Collections;

namespace VRInteraction
{

	public class DestroyIn : MonoBehaviour {

		public float seconds;
		public bool disableOnly;
		public Transform reParent;

		private float elapsedTime = 0;

		void OnEnable()
		{
			elapsedTime = 0;
		}

		void Update () 
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > seconds)
			{
				Destroy();
			}
		}

		public void Destroy()
		{
			if (disableOnly)
			{
				if (reParent != null) transform.parent = reParent;
				gameObject.SetActive(false);
			} else
				Destroy(gameObject);
		}
	}
}
