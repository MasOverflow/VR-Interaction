using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class DependencyChecker : EditorWindow
{
	private const string SteamVRDefine = "Int_SteamVR";
	private const string OculusDefine = "Int_Oculus";
	private const string VRInteractionDefine = "VRInteraction";

	[DidReloadScripts]
	private static void CheckVRPlatforms()
	{
		bool hasOculusSDK = DoesTypeExist("OVRInput");
		bool hasSteamVR = DoesTypeExist("SteamVR");

		string scriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		string[] scriptingDefines = scriptingDefine.Split(';');
		bool hasOculusSDKDefine = scriptingDefines.Contains(OculusDefine);
		bool hasSteamVRDefine = scriptingDefines.Contains(SteamVRDefine);
		bool hasVRInteractionDefine = scriptingDefine.Contains(VRInteractionDefine);

		string action = "";
		bool doingNothing = true;

		if (!hasVRInteractionDefine)
		{
			AddDefine(VRInteractionDefine);
			doingNothing = false;
			action += "Adding VRInteraction ";
		}

		if (hasOculusSDK && !hasOculusSDKDefine)
		{
			AddDefine(OculusDefine);
			doingNothing = false;
			action += "Adding Oculus ";
		} else if (!hasOculusSDK && hasOculusSDKDefine)
		{
			action += "Removing Oculus ";
			doingNothing = false;
			RemoveDefine(OculusDefine);
		}
			
		if (hasSteamVR && !hasSteamVRDefine)
		{
			AddDefine(SteamVRDefine);
			doingNothing = false;
			action += " Adding Steamvr ";
		} else if (!hasSteamVR && hasSteamVRDefine)
		{
			RemoveDefine(SteamVRDefine);
			doingNothing = false;
			action += " Removing Steamvr ";
		}
		if (doingNothing)
		{
			ClearProgressBar();
		}
		if (action != "") Debug.Log(action);
		if (!hasOculusSDK && !hasSteamVR)
		{
			EditorWindow.GetWindow(typeof(DependencyChecker), true, "VR Dependency", true);
		}
	}

	void OnGUI()
	{
		EditorGUILayout.HelpBox("This asset requires either SteamVR and or Oculus Integration to work. " +
			"Please download and import one or both from the asset store to continue", MessageType.Info);
		if (GUILayout.Button("SteamVR"))
		{
			Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647");
		}
		if (GUILayout.Button("Oculus Integration"))
		{
			Application.OpenURL("https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022");
		}
	}

	static private bool DoesTypeExist(string className)
	{
		var foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			where type.Name == className
			select type).FirstOrDefault();

		return foundType != null;
	}

	static private void RemoveDefine(string define)
	{
		DisplayProgressBar("Removing support for " + define);

		string scriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		string[] scriptingDefines = scriptingDefine.Split(';');
		List<string> listDefines = scriptingDefines.ToList();
		listDefines.Remove(define);

		string newDefines = string.Join(";", listDefines.ToArray());
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);
	}

	static private void AddDefine(string define)
	{
		DisplayProgressBar("Setting up support for " + define);

		string scriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		string[] scriptingDefines = scriptingDefine.Split(';');
		List<string> listDefines = scriptingDefines.ToList();
		listDefines.Add(define);

		string newDefines = string.Join(";", listDefines.ToArray());
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);

		if (PlayerSettings.virtualRealitySupported == false)
		{
			PlayerSettings.virtualRealitySupported = true;
		}
	}

	static private void DisplayProgressBar(string newMessage = "")
	{
		EditorUtility.DisplayProgressBar("VRInteraction", newMessage, UnityEngine.Random.value);
	}

	static private void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
}
