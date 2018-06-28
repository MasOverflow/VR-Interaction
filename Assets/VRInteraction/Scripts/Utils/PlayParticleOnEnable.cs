//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Plays the attached ParticleSystem on enable
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{

	public class PlayParticleOnEnable : MonoBehaviour 
	{
		void OnEnable()
		{
			ParticleSystem ps = GetComponent<ParticleSystem>();
			if (ps != null) 
			{
				ps.Clear(true);
				ps.Play();
			}
		}
	}
}