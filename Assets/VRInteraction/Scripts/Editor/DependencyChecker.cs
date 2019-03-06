using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class DependencyChecker : EditorWindow
{
	private const string SteamVRDefine = "Int_SteamVR";
	private const string SteamVR2Define = "Int_SteamVR2";
	private const string OculusDefine = "Int_Oculus";
	private const string VRInteractionDefine = "VRInteraction";

	[DidReloadScripts]
	private static void CheckVRPlatforms()
	{
		bool hasOculusSDK = DoesTypeExist("OVRInput");
		bool hasSteamVR = DoesTypeExist("SteamVR");
		bool hasSteamVR2 = hasSteamVR ? !DoesTypeExist("SteamVR_TrackedController") : false;

		string scriptingDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
		string[] scriptingDefines = scriptingDefine.Split(';');
		bool hasOculusSDKDefine = scriptingDefines.Contains(OculusDefine);
		bool hasSteamVRDefine = scriptingDefines.Contains(SteamVRDefine);
		bool hasSteamVR2Define = scriptingDefines.Contains(SteamVR2Define);
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
		if (hasSteamVR)
		{
			if (!hasSteamVRDefine)
			{
				AddDefine(SteamVRDefine);
				doingNothing = false;
				action += " Adding Steamvr ";
			}
			if (hasSteamVR2 && !hasSteamVR2Define)
			{
				AddDefine(SteamVR2Define);
				doingNothing = false;
				action += " Adding Steamvr 2";
				Debug.LogError("SteamVR 2 is not currently supported. You can download SteamVR 1.2.3 from here:\n" +
					"https://github.com/ValveSoftware/steamvr_unity_plugin/tree/fad02abee8ed45791993e92e420b340f63940aca\n" +
					"Please delete the SteamVR folder and replace with the one from this repo.");
			}
		} else if (!hasSteamVR && hasSteamVRDefine)
		{
			RemoveDefine(SteamVRDefine);
			if (hasSteamVR2Define) RemoveDefine(SteamVR2Define);
			doingNothing = false;
			action += " Removing Steamvr ";
		}
		if (!hasSteamVR2 && hasSteamVR2Define)
		{
			RemoveDefine(SteamVR2Define);
			doingNothing = false;
			action += " Removing Steamvr2 ";
		}
		if (doingNothing)
		{
			ClearProgressBar();
		} else
		{
			string weaponFolderPath = "Assets/VRWeaponInteractor";
			if (AssetDatabase.IsValidFolder(weaponFolderPath)) AssetDatabase.ImportAsset(weaponFolderPath, ImportAssetOptions.ImportRecursive);	
			string teleportFolderPath = "Assets/VRArcTeleporter";
			if (AssetDatabase.IsValidFolder(teleportFolderPath)) AssetDatabase.ImportAsset(teleportFolderPath, ImportAssetOptions.ImportRecursive);
			string userInterfaceFolderPath = "Assets/VRUserInterfaces";
			if (AssetDatabase.IsValidFolder(userInterfaceFolderPath)) AssetDatabase.ImportAsset(userInterfaceFolderPath, ImportAssetOptions.ImportRecursive);
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
