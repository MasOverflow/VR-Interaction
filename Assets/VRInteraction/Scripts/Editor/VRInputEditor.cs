//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRInput
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#if Int_SteamVR
using Valve.VR;
#endif

namespace VRInteraction
{
	[CustomEditor(typeof(VRInput))]
	public class VRInputEditor : Editor 
	{
		// target component
		public VRInput input = null;

		#if Int_Oculus || (Int_SteamVR && !Int_SteamVR2)
		static bool editActionsFoldout;
		string newActionName = "";
		protected bool lockToOculus;
		#endif

		public virtual void OnEnable()
		{
			input = (VRInput)target;
			#if Int_Oculus || (Int_SteamVR && !Int_SteamVR2)
			lockToOculus = !input.isSteamVR();
			#endif
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (input.VRActions == null || input.VRActions.Length == 0)
			{
				ResetToInteractbaleDefault();
			}

			if (GUILayout.Button("Reset To Interactable Default"))
			{
				ResetToInteractbaleDefault();
			}

			GUIStyle titleStyle = new GUIStyle();
			titleStyle.fontSize = 24;
			titleStyle.normal.textColor = Color.white;

			#if Int_Oculus || (Int_SteamVR && !Int_SteamVR2)

			string title1TitleString = "";
			bool hasOculus = false;
			#if Int_Oculus
			title1TitleString += "Oculus ";
			hasOculus = true;
			#endif
			bool hasLegacySteamVR = false;
			#if (Int_SteamVR && !Int_SteamVR2)
			title1TitleString += "and SteamVR Legacy";
			hasLegacySteamVR = true;
			#endif

			if ((input.isSteamVR() && hasLegacySteamVR) || (!input.isSteamVR() && hasOculus))
			{
				GUIContent title1Content = new GUIContent(title1TitleString);
				float height = titleStyle.CalcHeight(title1Content, 10f);
				EditorGUILayout.LabelField(title1Content, titleStyle, GUILayout.Height(height));

				editActionsFoldout = EditorGUILayout.Foldout(editActionsFoldout, "Edit Actions");

				if (editActionsFoldout)
				{
					if (input.VRActions != null)
					{
						for(int i=0; i<input.VRActions.Length; i++)
						{
							EditorGUILayout.BeginHorizontal();
							input.VRActions[i] = EditorGUILayout.TextField(input.VRActions[i]);
							if (GUILayout.Button("X"))
							{
								string[] newActions = new string[input.VRActions.Length-1];
								int offset = 0;
								for(int j=0; j<newActions.Length; j++)
								{
									if (i == j) offset = 1;
									newActions[j] = input.VRActions[j+offset];
								}
								input.VRActions = newActions;

								if (input.triggerKey > i)
									input.triggerKey -= 1;
								else if (input.triggerKey == i)
									input.triggerKey = 0;
                                for(int j=0; j<input.triggerKeys.Count;j++)
                                {
                                    if (input.triggerKeys[j] > i)
                                        input.triggerKeys[j] -= 1;
                                    else if (input.triggerKeys[j] == i)
                                        input.triggerKeys[j] = 0;
                                }
								if (input.padTop > i)
									input.padTop -= 1;
								else if (input.padTop == i)
									input.padTop = 0;

                                for (int j = 0; j < input.padTops.Count; j++)
                                {
                                    if (input.padTops[j] > i)
                                        input.padTops[j] -= 1;
                                    else if (input.padTops[j] == i)
                                        input.padTops[j] = 0;
                                }

                                if (input.padLeft > i)
									input.padLeft -= 1;
								else if (input.padLeft == i)
									input.padLeft = 0;

                                for (int j = 0; j < input.padLefts.Count; j++)
                                {
                                    if (input.padLefts[j] > i)
                                        input.padLefts[j] -= 1;
                                    else if (input.padLefts[j] == i)
                                        input.padLefts[j] = 0;
                                }

                                if (input.padRight > i)
									input.padRight -= 1;
								else if (input.padRight == i)
									input.padRight = 0;

                                for (int j = 0; j < input.padRights.Count; j++)
                                {
                                    if (input.padRights[j] > i)
                                        input.padRights[j] -= 1;
                                    else if (input.padRights[j] == i)
                                        input.padRights[j] = 0;
                                }

                                if (input.padBottom > i)
									input.padBottom -= 1;
								else if (input.padBottom == i)
									input.padBottom = 0;

                                for (int j = 0; j < input.padBottoms.Count; j++)
                                {
                                    if (input.padBottoms[j] > i)
                                        input.padBottoms[j] -= 1;
                                    else if (input.padBottoms[j] == i)
                                        input.padBottoms[j] = 0;
                                }

                                if (input.padCentre > i)
									input.padCentre -= 1;
								else if (input.padCentre == i)								
									input.padTouch = 0;

                                for (int j = 0; j < input.padCentres.Count; j++)
                                {
                                    if (input.padCentres[j] > i)
                                        input.padCentres[j] -= 1;
                                    else if (input.padCentres[j] == i)
                                        input.padCentres[j] = 0;
                                }

                                if (input.padTouch > i)
									input.padTouch -= 1;
								else if (input.padTouch == i)
									input.padTouch = 0;

                                for (int j = 0; j < input.padTouchs.Count; j++)
                                {
                                    if (input.padTouchs[j] > i)
                                        input.padTouchs[j] -= 1;
                                    else if (input.padTouchs[j] == i)
                                        input.padTouchs[j] = 0;
                                }

                                if (input.gripKey > i)
									input.gripKey -= 1;
								else if (input.gripKey == i)
									input.gripKey = 0;

                                for (int j = 0; j < input.gripKeys.Count; j++)
                                {
                                    if (input.gripKeys[j] > i)
                                        input.gripKeys[j] -= 1;
                                    else if (input.gripKeys[j] == i)
                                        input.gripKeys[j] = 0;
                                }

                                if (input.menuKey > i)
									input.menuKey -= 1;
								else if (input.menuKey == i)
									input.menuKey = 0;

                                for (int j = 0; j < input.menuKeys.Count; j++)
                                {
                                    if (input.menuKeys[j] > i)
                                        input.menuKeys[j] -= 1;
                                    else if (input.menuKeys[j] == i)
                                        input.menuKeys[j] = 0;
                                }

                                if (input.AXKey > i)
									input.AXKey -= 1;
								else if (input.AXKey == i)
									input.AXKey = 0;

                                for (int j = 0; j < input.AXKeys.Count; j++)
                                {
                                    if (input.AXKeys[j] > i)
                                        input.AXKeys[j] -= 1;
                                    else if (input.AXKeys[j] == i)
                                        input.AXKeys[j] = 0;
                                }

                                if (input.triggerKeyOculus > i)
									input.triggerKeyOculus -= 1;
								else if (input.triggerKeyOculus == i)
									input.triggerKeyOculus = 0;

                                for (int j = 0; j < input.triggerKeysOculus.Count; j++)
                                {
                                    if (input.triggerKeysOculus[j] > i)
                                        input.triggerKeysOculus[j] -= 1;
                                    else if (input.triggerKeysOculus[j] == i)
                                        input.triggerKeysOculus[j] = 0;
                                }

                                if (input.padTopOculus > i)
									input.padTopOculus -= 1;
								else if (input.padTopOculus == i)
									input.padTopOculus = 0;

                                for (int j = 0; j < input.padTopsOculus.Count; j++)
                                {
                                    if (input.padTopsOculus[j] > i)
                                        input.padTopsOculus[j] -= 1;
                                    else if (input.padTopsOculus[j] == i)
                                        input.padTopsOculus[j] = 0;
                                }

                                if (input.padLeftOculus > i)
									input.padLeftOculus -= 1;
								else if (input.padLeftOculus == i)
									input.padLeftOculus = 0;

                                for (int j = 0; j < input.padLeftsOculus.Count; j++)
                                {
                                    if (input.padLeftsOculus[j] > i)
                                        input.padLeftsOculus[j] -= 1;
                                    else if (input.padLeftsOculus[j] == i)
                                        input.padLeftsOculus[j] = 0;
                                }

                                if (input.padRightOculus > i)
									input.padRightOculus -= 1;
								else if (input.padRightOculus == i)
									input.padRightOculus = 0;

                                for (int j = 0; j < input.padRightsOculus.Count; j++)
                                {
                                    if (input.padRightsOculus[j] > i)
                                        input.padRightsOculus[j] -= 1;
                                    else if (input.padRightsOculus[j] == i)
                                        input.padRightsOculus[j] = 0;
                                }

                                if (input.padBottomOculus > i)
									input.padBottomOculus -= 1;
								else if (input.padBottomOculus == i)
									input.padBottomOculus = 0;

                                for (int j = 0; j < input.padBottomsOculus.Count; j++)
                                {
                                    if (input.padBottomsOculus[j] > i)
                                        input.padBottomsOculus[j] -= 1;
                                    else if (input.padBottomsOculus[j] == i)
                                        input.padBottomsOculus[j] = 0;
                                }

                                if (input.padCentreOculus > i)
									input.padCentreOculus -= 1;
								else if (input.padCentreOculus == i)
									input.padCentreOculus = 0;

                                for (int j = 0; j < input.padCentresOculus.Count; j++)
                                {
                                    if (input.padCentresOculus[j] > i)
                                        input.padCentresOculus[j] -= 1;
                                    else if (input.padCentresOculus[j] == i)
                                        input.padCentresOculus[j] = 0;
                                }

                                if (input.padTouchOculus > i)
									input.padTouchOculus -= 1;
								else if (input.padTouchOculus == i)
									input.padTouchOculus = 0;

                                for (int j = 0; j < input.padTouchsOculus.Count; j++)
                                {
                                    if (input.padTouchsOculus[j] > i)
                                        input.padTouchsOculus[j] -= 1;
                                    else if (input.padTouchsOculus[j] == i)
                                        input.padTouchsOculus[j] = 0;
                                }

                                if (input.gripKeyOculus > i)
									input.gripKeyOculus -= 1;
								else if (input.gripKeyOculus == i)
									input.gripKeyOculus = 0;

                                for (int j = 0; j < input.gripKeysOculus.Count; j++)
                                {
                                    if (input.gripKeysOculus[j] > i)
                                        input.gripKeysOculus[j] -= 1;
                                    else if (input.gripKeysOculus[j] == i)
                                        input.gripKeysOculus[j] = 0;
                                }

                                if (input.menuKeyOculus > i)
									input.menuKeyOculus -= 1;
								else if (input.menuKeyOculus == i)
									input.menuKeyOculus = 0;

                                for (int j = 0; j < input.menuKeysOculus.Count; j++)
                                {
                                    if (input.menuKeysOculus[j] > i)
                                        input.menuKeysOculus[j] -= 1;
                                    else if (input.menuKeysOculus[j] == i)
                                        input.menuKeysOculus[j] = 0;
                                }

                                if (input.AXKeyOculus > i)
									input.AXKeyOculus -= 1;
								else if (input.AXKeyOculus == i)
									input.AXKeyOculus = 0;

                                for (int j = 0; j < input.AXKeysOculus.Count; j++)
                                {
                                    if (input.AXKeysOculus[j] > i)
                                        input.AXKeysOculus[j] -= 1;
                                    else if (input.AXKeysOculus[j] == i)
                                        input.AXKeysOculus[j] = 0;
                                }

                                EditorUtility.SetDirty(input);
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
								break;
							}
							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.BeginHorizontal();
					newActionName = EditorGUILayout.TextField(newActionName);
					GUI.enabled = (newActionName != "");
					if (GUILayout.Button("Add Action"))
					{
						string[] newActions = new string[1];
						if (input.VRActions != null) newActions = new string[input.VRActions.Length+1];
						else input.VRActions = new string[0];
						for(int i=0; i<newActions.Length; i++)
						{
							if (i == input.VRActions.Length)
							{
								newActions[i] = newActionName;
								break;
							}
							newActions[i] = input.VRActions[i];
						}
						input.VRActions = newActions;
						newActionName = "";
						EditorUtility.SetDirty(input);
						EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
				}

				if (input.VRActions == null)
				{
					serializedObject.ApplyModifiedProperties();
					return;
				}

				SerializedProperty triggerKey = serializedObject.FindProperty("triggerKey");
				SerializedProperty padTop = serializedObject.FindProperty("padTop");
				SerializedProperty padLeft = serializedObject.FindProperty("padLeft");
				SerializedProperty padRight = serializedObject.FindProperty("padRight");
				SerializedProperty padBottom = serializedObject.FindProperty("padBottom");
				SerializedProperty padCentre = serializedObject.FindProperty("padCentre");
				SerializedProperty padTouch = serializedObject.FindProperty("padTouch");
				SerializedProperty gripKey = serializedObject.FindProperty("gripKey");
				SerializedProperty menuKey = serializedObject.FindProperty("menuKey");
				SerializedProperty AXKey = serializedObject.FindProperty("AXKey");
				SerializedProperty triggerKeyOculus = serializedObject.FindProperty("triggerKeyOculus");
				SerializedProperty padTopOculus = serializedObject.FindProperty("padTopOculus");
				SerializedProperty padLeftOculus = serializedObject.FindProperty("padLeftOculus");
				SerializedProperty padRightOculus = serializedObject.FindProperty("padRightOculus");
				SerializedProperty padBottomOculus = serializedObject.FindProperty("padBottomOculus");
				SerializedProperty padCentreOculus = serializedObject.FindProperty("padCentreOculus");
				SerializedProperty padTouchOculus = serializedObject.FindProperty("padTouchOculus");
				SerializedProperty gripKeyOculus = serializedObject.FindProperty("gripKeyOculus");
				SerializedProperty menuKeyOculus = serializedObject.FindProperty("menuKeyOculus");
				SerializedProperty AXKeyOculus = serializedObject.FindProperty("AXKeyOculus");

                SerializedProperty triggerKeys = serializedObject.FindProperty("triggerKeys");
                SerializedProperty padTops = serializedObject.FindProperty("padTops");
                SerializedProperty padLefts = serializedObject.FindProperty("padLefts");
                SerializedProperty padRights = serializedObject.FindProperty("padRights");
                SerializedProperty padBottoms = serializedObject.FindProperty("padBottoms");
                SerializedProperty padCentres = serializedObject.FindProperty("padCentres");
                SerializedProperty padTouchs = serializedObject.FindProperty("padTouchs");
                SerializedProperty gripKeys = serializedObject.FindProperty("gripKeys");
                SerializedProperty menuKeys = serializedObject.FindProperty("menuKeys");
                SerializedProperty AXKeys = serializedObject.FindProperty("AXKeys");
                SerializedProperty triggerKeysOculus = serializedObject.FindProperty("triggerKeysOculus");
                SerializedProperty padTopsOculus = serializedObject.FindProperty("padTopsOculus");
                SerializedProperty padLeftsOculus = serializedObject.FindProperty("padLeftsOculus");
                SerializedProperty padRightsOculus = serializedObject.FindProperty("padRightsOculus");
                SerializedProperty padBottomsOculus = serializedObject.FindProperty("padBottomsOculus");
                SerializedProperty padCentresOculus = serializedObject.FindProperty("padCentresOculus");
                SerializedProperty padTouchsOculus = serializedObject.FindProperty("padTouchsOculus");
                SerializedProperty gripKeysOculus = serializedObject.FindProperty("gripKeysOculus");
                SerializedProperty menuKeysOculus = serializedObject.FindProperty("menuKeysOculus");
                SerializedProperty AXKeysOculus = serializedObject.FindProperty("AXKeysOculus");

                SerializedProperty displayViveButtons = serializedObject.FindProperty("displayViveButtons");
				SerializedProperty mirrorControls = serializedObject.FindProperty("mirrorControls");
				if (!lockToOculus)
				{
					GUIContent viveDisplayModeText = new GUIContent("Display Vive Buttons", "Or Oculus Buttons When Set To False");
					displayViveButtons.boolValue = EditorGUILayout.Toggle(viveDisplayModeText, displayViveButtons.boolValue);

					GUIContent mirrorControlsText = new GUIContent("Mirror Controls", "If Set To False Will Seperate Oculus And Vive Controls");
					mirrorControls.boolValue = EditorGUILayout.Toggle(mirrorControlsText, mirrorControls.boolValue);
				} else
				{
					mirrorControls.boolValue = true;
					displayViveButtons.boolValue = false;
				}

				if (!mirrorControls.boolValue)
				{
					int newTriggerKey = EditorGUILayout.Popup("Trigger Key", displayViveButtons.boolValue ? triggerKey.intValue : triggerKeyOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) triggerKey.intValue = newTriggerKey;
					else triggerKeyOculus.intValue = newTriggerKey;

                    AdditionalButtonArrayUnMirrored(triggerKeys, triggerKeysOculus, "Trigger Key", "Trigger Key", displayViveButtons.boolValue);

                    int newPadTop = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Up Key" : "Thumbstick Up"), displayViveButtons.boolValue ? padTop.intValue : padTopOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padTop.intValue = newPadTop;
					else padTopOculus.intValue = newPadTop;

                    AdditionalButtonArrayUnMirrored(padTops, padTopsOculus, "Pad Up Key", "Thumbstick Up", displayViveButtons.boolValue);

                    int newPadLeft = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Left Key" : "Thumbstick Left"), displayViveButtons.boolValue ? padLeft.intValue : padLeftOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padLeft.intValue = newPadLeft;
					else padLeftOculus.intValue = newPadLeft;

                    AdditionalButtonArrayUnMirrored(padLefts, padLeftsOculus, "Pad Left Key", "Thumbstick Left", displayViveButtons.boolValue);

                    int newPadRight = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Right Key" : "Thumbstick Right"), displayViveButtons.boolValue ? padRight.intValue : padRightOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padRight.intValue = newPadRight;
					else padRightOculus.intValue = newPadRight;

                    AdditionalButtonArrayUnMirrored(padRights, padRightsOculus, "Pad Right Key", "Thumbstick Right", displayViveButtons.boolValue);

                    int newPadBottom = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Down Key" : "Thumbstick Down"), displayViveButtons.boolValue ? padBottom.intValue : padBottomOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padBottom.intValue = newPadBottom;
					else padBottomOculus.intValue = newPadBottom;

                    AdditionalButtonArrayUnMirrored(padBottoms, padBottomsOculus, "Pad Down Key", "Thumbstick Down", displayViveButtons.boolValue);

                    int newPadCentre = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Centre Key" : "Thumbstick Button"), displayViveButtons.boolValue ? padCentre.intValue : padCentreOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padCentre.intValue = newPadCentre;
					else padCentreOculus.intValue = newPadCentre;

                    AdditionalButtonArrayUnMirrored(padCentres, padCentresOculus, "Pad Centre Key", "Thumbstick Button", displayViveButtons.boolValue);

                    int newPadTouch = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Touch Key" : "Thumbstick Touch"), displayViveButtons.boolValue ? padTouch.intValue : padTouchOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) padTouch.intValue = newPadTouch;
					else padTouchOculus.intValue = newPadTouch;

                    AdditionalButtonArrayUnMirrored(padTouchs, padTouchsOculus, "Pad Touch Key", "Thumbstick Touch", displayViveButtons.boolValue);

                    int newGripKey = EditorGUILayout.Popup("Grip Key", displayViveButtons.boolValue ? gripKey.intValue : gripKeyOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) gripKey.intValue = newGripKey;
					else gripKeyOculus.intValue = newGripKey;

                    AdditionalButtonArrayUnMirrored(gripKeys, gripKeysOculus, "Grip Key", "Grip Key", displayViveButtons.boolValue);

                    int newMenuKey = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Menu Key" : "B/Y"), displayViveButtons.boolValue ? menuKey.intValue : menuKeyOculus.intValue, input.VRActions);
					if (displayViveButtons.boolValue) menuKey.intValue = newMenuKey;
					else menuKeyOculus.intValue = newMenuKey;

                    AdditionalButtonArrayUnMirrored(menuKeys, menuKeysOculus, "Menu Key", "B/Y", displayViveButtons.boolValue);

                    if (!displayViveButtons.boolValue)
                    {
                        AXKeyOculus.intValue = EditorGUILayout.Popup("A/X", AXKeyOculus.intValue, input.VRActions);

                        AdditionalButtonArrayUnMirrored(AXKeys, AXKeysOculus, "A/X", "A/X", displayViveButtons.boolValue);
                    }

				} else
				{
					triggerKey.intValue = EditorGUILayout.Popup("Trigger Key", triggerKey.intValue, input.VRActions);
					triggerKeyOculus.intValue = triggerKey.intValue;
                    AdditionalButtonArray(triggerKeys, triggerKeysOculus, "Trigger Key");
                    padTop.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Up Key" : "Thumbstick Up", padTop.intValue, input.VRActions);
					padTopOculus.intValue = padTop.intValue;

                    AdditionalButtonArray(padTops, padTopsOculus, displayViveButtons.boolValue ? "Pad Up Key" : "Thumbstick Up");

                    padLeft.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Left Key" : "Thumbstick Left", padLeft.intValue, input.VRActions);
					padLeftOculus.intValue = padLeft.intValue;

                    AdditionalButtonArray(padLefts, padLeftsOculus, displayViveButtons.boolValue ? "Pad Left Key" : "Thumbstick Left");

                    padRight.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Right Key" : "Thumbstick Right", padRight.intValue, input.VRActions);
					padRightOculus.intValue = padRight.intValue;

                    AdditionalButtonArray(padRights, padRightsOculus, displayViveButtons.boolValue ? "Pad Right Key" : "Thumbstick Right");

                    padBottom.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Down Key" : "Thumbstick Down", padBottom.intValue, input.VRActions);
					padBottomOculus.intValue = padBottom.intValue;

                    AdditionalButtonArray(padBottoms, padBottomsOculus, displayViveButtons.boolValue ? "Pad Down Key" : "Thumbstick Down");

                    padCentre.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Centre Key" : "Thumbstick Button", padCentre.intValue, input.VRActions);
					padCentreOculus.intValue = padCentre.intValue;

                    AdditionalButtonArray(padCentres, padCentresOculus, displayViveButtons.boolValue ? "Pad Centre Key" : "Thumbstick Button");

                    padTouch.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Pad Touch Key" : "Thumbstick Touch", padTouch.intValue, input.VRActions);
					padTouchOculus.intValue = padTouch.intValue;

                    AdditionalButtonArray(padTouchs, padTouchsOculus, displayViveButtons.boolValue ? "Pad Touch Key" : "Thumbstick Touch");

                    gripKey.intValue = EditorGUILayout.Popup("Grip Key", gripKey.intValue, input.VRActions);
					gripKeyOculus.intValue = gripKey.intValue;

                    AdditionalButtonArray(gripKeys, gripKeysOculus, "Grip Key");

                    menuKey.intValue = EditorGUILayout.Popup(displayViveButtons.boolValue ? "Menu Key" : "B/Y", menuKey.intValue, input.VRActions);
					menuKeyOculus.intValue = menuKey.intValue;

                    AdditionalButtonArray(menuKeys, menuKeysOculus, displayViveButtons.boolValue ? "Menu Key" : "B/Y");

                    if (!displayViveButtons.boolValue)
					{
						AXKey.intValue = EditorGUILayout.Popup("A/X", AXKey.intValue, input.VRActions);
						AXKeyOculus.intValue = AXKey.intValue;

                        AdditionalButtonArray(AXKeys, AXKeysOculus, "A/X");

                    }
				}

				#if Int_Oculus
				if (!input.isSteamVR())
				{
					SerializedProperty controllerHand = serializedObject.FindProperty("controllerHand");
					EditorGUILayout.PropertyField(controllerHand);
				}
				#endif

				EditorGUILayout.HelpBox("The VRInput script allows you to specify a list of custom actions. " +
				"Do this by expanding the 'Edit Actions' foldout and adding or removing from the list, " +
				"You can then assign the actions to controller keys.\n" +
				"The method 'InputReceived' is called on this object using a SendMessage call. You can implement this " +
				"method in any script on this object.", MessageType.Info);
				
			}

			#endif
			#if Int_SteamVR2

			if (input.isSteamVR())
			{
				GUIContent title2Content = new GUIContent("SteamVR 2.0");
				float height2 = titleStyle.CalcHeight(title2Content, 10f);
				EditorGUILayout.LabelField(title2Content, titleStyle, GUILayout.Height(height2));

				SerializedProperty handType = serializedObject.FindProperty("handType");
				EditorGUILayout.PropertyField(handType);

				SerializedProperty triggerPressure = serializedObject.FindProperty("triggerPressure");
				EditorGUILayout.PropertyField(triggerPressure);

				SerializedProperty touchPosition = serializedObject.FindProperty("touchPosition");
				EditorGUILayout.PropertyField(touchPosition);

				SerializedProperty padTouched = serializedObject.FindProperty("padTouched");
				EditorGUILayout.PropertyField(padTouched);

				SerializedProperty padPressed = serializedObject.FindProperty("padPressed");
				EditorGUILayout.PropertyField(padPressed);

				SerializedProperty booleanActions = serializedObject.FindProperty("booleanActions");
				EditorGUILayout.PropertyField(booleanActions, true);

				EditorGUILayout.HelpBox("Create your actions in the SteamVR Input Editor. Then specify " +
					"the actions in the lists above. The name of the action is the method called.\n" +
					"The method 'InputReceived' is called on this object using a SendMessage call. You can implement this " +
					"method in any script on this object.", MessageType.Info);
			}

			#endif

			serializedObject.ApplyModifiedProperties();
		}

        private void AdditionalButtonArray(SerializedProperty actionKeys, SerializedProperty actionsKeysOculus, string displayName)
        {
            bool changed = false;
            for (int x = 0; x < actionKeys.arraySize; x++)
            {
                SerializedProperty property = actionKeys.GetArrayElementAtIndex(x);
                int oldProperty = property.intValue;
                GUILayout.BeginHorizontal();
                property.intValue = EditorGUILayout.Popup(displayName, property.intValue, input.VRActions);
                if (oldProperty != property.intValue) changed = true;

                if (GUILayout.Button("-"))
                {
                    actionKeys.DeleteArrayElementAtIndex(x);
                    changed = true;
                    break;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                actionKeys.arraySize++;
                changed = true;
            }
            GUILayout.EndHorizontal();
            if (changed)
            {
                actionsKeysOculus.arraySize = actionKeys.arraySize;
                for (int x = 0; x < actionKeys.arraySize; x++)
                {
                    SerializedProperty property = actionKeys.GetArrayElementAtIndex(x);
                    SerializedProperty property2 = actionsKeysOculus.GetArrayElementAtIndex(x);
                    property2.intValue = property.intValue;
                }
            }
        }

        private void AdditionalButtonArrayUnMirrored(SerializedProperty actionKeys, SerializedProperty actionsKeysOculus, string displayName, string displayNameOculus, bool displayVive)
        {
            if (displayVive)
            {
                for (int x = 0; x < actionKeys.arraySize; x++)
                {
                    SerializedProperty property = actionKeys.GetArrayElementAtIndex(x);
                    GUILayout.BeginHorizontal();
                    property.intValue = EditorGUILayout.Popup(displayName, property.intValue, input.VRActions);
                    if (GUILayout.Button("-"))
                    {
                        actionKeys.DeleteArrayElementAtIndex(x);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+"))
                {
                    actionKeys.arraySize++;
                }
            }
            else
            {
                for (int x = 0; x < actionsKeysOculus.arraySize; x++)
                {
                    SerializedProperty property = actionsKeysOculus.GetArrayElementAtIndex(x);
                    GUILayout.BeginHorizontal();
                    property.intValue = EditorGUILayout.Popup(displayNameOculus, property.intValue, input.VRActions);
                    if (GUILayout.Button("-"))
                    {
                        actionsKeysOculus.DeleteArrayElementAtIndex(x);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+"))
                {
                    actionsKeysOculus.arraySize++;
                }
            }
        }

        public void ResetToInteractbaleDefault()
		{
			input.VRActions = new string[] { "NONE", "ACTION", "PICKUP_DROP" };
			#if Int_Oculus || (Int_SteamVR && !Int_SteamVR2)

			input.triggerKey = 1;
			input.triggerKeyOculus = 1;
			input.padTop = 0;
			input.padTopOculus = 0;
			input.padLeft = 0;
			input.padLeftOculus = 0;
			input.padRight = 0;
			input.padRightOculus = 0;
			input.padBottom = 0;
			input.padBottomOculus = 0;
			input.padCentre = 0;
			input.padCentreOculus = 0;
			input.padTouch = 0;
			input.padTouchOculus = 0;
			input.gripKey = 2;
			input.gripKeyOculus = 2;
			input.menuKey = 0;
			input.menuKeyOculus = 0;
			input.AXKey = 0;
			input.AXKeyOculus = 0;
			#endif

			#if Int_SteamVR2
			if (input.isSteamVR())
			{
				SteamVR_Action_Boolean actionAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ACTION");
				SteamVR_Action_Boolean pickupDropAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PICKUP_DROP");
				input.booleanActions.Clear();
				input.booleanActions.Add(actionAction);
				input.booleanActions.Add(pickupDropAction);

				input.triggerPressure = SteamVR_Input.GetAction<SteamVR_Action_Single>("TriggerPressure");
				input.touchPosition = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("TouchPosition");
				input.padTouched = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadTouched");
				input.padPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PadPressed");

				input.handType = input.LeftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
				SteamVR_Behaviour_Pose poseComp = input.GetComponent<SteamVR_Behaviour_Pose>();
				if (poseComp == null)
				{
					poseComp = input.gameObject.AddComponent<SteamVR_Behaviour_Pose>();
					poseComp.inputSource = input.handType;
				}
			}

			#endif

			EditorUtility.SetDirty(input);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}

}
