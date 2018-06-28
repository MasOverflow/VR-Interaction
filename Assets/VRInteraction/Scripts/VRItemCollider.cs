//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Used for forwarding on trigger collision to a parent item
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class VRItemCollider : MonoBehaviour 
	{
		public VRInteractableItem item;

		private Collider _col;

		public Collider col
		{
			get 
			{
				if (_col == null) Awake();
				return _col; 
			}
		}

		void Awake()
		{
			_col = GetComponent<Collider>();
		}
	}
}