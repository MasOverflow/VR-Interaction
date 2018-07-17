using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EnableOnHMD : MonoBehaviour {

	public List<GameObject> steamVREnable = new List<GameObject>();
	public List<GameObject> oculusEnable = new List<GameObject>();

	void Start () 
	{
		if (!XRSettings.enabled) return;

		bool steamVR = XRSettings.loadedDeviceName == "OpenVR";
		foreach(GameObject steamVRObj in steamVREnable)
		{
			if (steamVRObj == null) continue;
			steamVRObj.SetActive(steamVR);
		}
		foreach(GameObject oculusObj in oculusEnable)
		{
			if (oculusObj == null) continue;
			oculusObj.SetActive(!steamVR);
		}
	}
}
