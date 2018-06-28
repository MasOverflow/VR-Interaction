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

namespace VRInteraction
{
	[CustomEditor(typeof(VRInput))]
	public class VRInputEditor : Editor 
	{
		// target component
		public VRInput input = null;

		static bool editActionsFoldout;
		string newActionName = "";

		protected bool lockToOculus;

		public virtual void OnEnable()
		{
			input = (VRInput)target;
			lockToOculus = !input.isSteamVR();
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
							if (input.padTop > i)
								input.padTop -= 1;
							else if (input.padTop == i)
								input.padTop = 0;
							if (input.padLeft > i)
								input.padLeft -= 1;
							else if (input.padLeft == i)
								input.padLeft = 0;
							if (input.padRight > i)
								input.padRight -= 1;
							else if (input.padRight == i)
								input.padRight = 0;
							if (input.padBottom > i)
								input.padBottom -= 1;
							else if (input.padBottom == i)
								input.padBottom = 0;
							if (input.padCentre > i)
								input.padCentre -= 1;
							else if (input.padCentre == i)								
								input.padTouch = 0;
							if (input.padTouch > i)
								input.padTouch -= 1;
							else if (input.padTouch == i)
								input.padTouch = 0;
							if (input.gripKey > i)
								input.gripKey -= 1;
							else if (input.gripKey == i)
								input.gripKey = 0;
							if (input.menuKey > i)
								input.menuKey -= 1;
							else if (input.menuKey == i)
								input.menuKey = 0;
							if (input.AXKey > i)
								input.AXKey -= 1;
							else if (input.AXKey == i)
								input.AXKey = 0;

							if (input.triggerKeyOculus > i)
								input.triggerKeyOculus -= 1;
							else if (input.triggerKeyOculus == i)
								input.triggerKeyOculus = 0;
							if (input.padTopOculus > i)
								input.padTopOculus -= 1;
							else if (input.padTopOculus == i)
								input.padTopOculus = 0;
							if (input.padLeftOculus > i)
								input.padLeftOculus -= 1;
							else if (input.padLeftOculus == i)
								input.padLeftOculus = 0;
							if (input.padRightOculus > i)
								input.padRightOculus -= 1;
							else if (input.padRightOculus == i)
								input.padRightOculus = 0;
							if (input.padBottomOculus > i)
								input.padBottomOculus -= 1;
							else if (input.padBottomOculus == i)
								input.padBottomOculus = 0;
							if (input.padCentreOculus > i)
								input.padCentreOculus -= 1;
							else if (input.padCentreOculus == i)
								input.padCentreOculus = 0;
							if (input.padTouchOculus > i)
								input.padTouchOculus -= 1;
							else if (input.padTouchOculus == i)
								input.padTouchOculus = 0;
							if (input.gripKeyOculus > i)
								input.gripKeyOculus -= 1;
							else if (input.gripKeyOculus == i)
								input.gripKeyOculus = 0;
							if (input.menuKeyOculus > i)
								input.menuKeyOculus -= 1;
							else if (input.menuKeyOculus == i)
								input.menuKeyOculus = 0;
							if (input.AXKeyOculus > i)
								input.AXKeyOculus -= 1;
							else if (input.AXKeyOculus == i)
								input.AXKeyOculus = 0;

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
				int newPadTop = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Up Key" : "Thumbstick Up"), displayViveButtons.boolValue ? padTop.intValue : padTopOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padTop.intValue = newPadTop;
				else padTopOculus.intValue = newPadTop;
				int newPadLeft = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Left Key" : "Thumbstick Left"), displayViveButtons.boolValue ? padLeft.intValue : padLeftOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padLeft.intValue = newPadLeft;
				else padLeftOculus.intValue = newPadLeft;
				int newPadRight = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Right Key" : "Thumbstick Right"), displayViveButtons.boolValue ? padRight.intValue : padRightOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padRight.intValue = newPadRight;
				else padRightOculus.intValue = newPadRight;
				int newPadBottom = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Down Key" : "Thumbstick Down"), displayViveButtons.boolValue ? padBottom.intValue : padBottomOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padBottom.intValue = newPadBottom;
				else padBottomOculus.intValue = newPadBottom;
				int newPadCentre = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Centre Key" : "Thumbstick Button"), displayViveButtons.boolValue ? padCentre.intValue : padCentreOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padCentre.intValue = newPadCentre;
				else padCentreOculus.intValue = newPadCentre;
				int newPadTouch = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Touch Key" : "Thumbstick Touch"), displayViveButtons.boolValue ? padTouch.intValue : padTouchOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) padTouch.intValue = newPadTouch;
				else padTouchOculus.intValue = newPadTouch;
				int newGripKey = EditorGUILayout.Popup("Grip Key", displayViveButtons.boolValue ? gripKey.intValue : gripKeyOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) gripKey.intValue = newGripKey;
				else gripKeyOculus.intValue = newGripKey;
				int newMenuKey = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Menu Key" : "B/Y"), displayViveButtons.boolValue ? menuKey.intValue : menuKeyOculus.intValue, input.VRActions);
				if (displayViveButtons.boolValue) menuKey.intValue = newMenuKey;
				else menuKeyOculus.intValue = newMenuKey;
				if (!displayViveButtons.boolValue) AXKeyOculus.intValue = EditorGUILayout.Popup("A/X", AXKeyOculus.intValue, input.VRActions);

			} else
			{
				triggerKey.intValue = EditorGUILayout.Popup("Trigger Key", triggerKey.intValue, input.VRActions);
				triggerKeyOculus.intValue = triggerKey.intValue;
				padTop.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Up Key" : "Thumbstick Up"), padTop.intValue, input.VRActions);
				padTopOculus.intValue = padTop.intValue;
				padLeft.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Left Key" : "Thumbstick Left"), padLeft.intValue, input.VRActions);
				padLeftOculus.intValue = padLeft.intValue;
				padRight.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Right Key" : "Thumbstick Right"), padRight.intValue, input.VRActions);
				padRightOculus.intValue = padRight.intValue;
				padBottom.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Down Key" : "Thumbstick Down"), padBottom.intValue, input.VRActions);
				padBottomOculus.intValue = padBottom.intValue;
				padCentre.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Centre Key" : "Thumbstick Button"), padCentre.intValue, input.VRActions);
				padCentreOculus.intValue = padCentre.intValue;
				padTouch.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Pad Touch Key" : "Thumbstick Touch"), padTouch.intValue, input.VRActions);
				padTouchOculus.intValue = padTouch.intValue;
				gripKey.intValue = EditorGUILayout.Popup("Grip Key", gripKey.intValue, input.VRActions);
				gripKeyOculus.intValue = gripKey.intValue;
				menuKey.intValue = EditorGUILayout.Popup((displayViveButtons.boolValue ? "Menu Key" : "B/Y"), menuKey.intValue, input.VRActions);
				menuKeyOculus.intValue = menuKey.intValue;
				if (!displayViveButtons.boolValue)
				{
					AXKey.intValue = EditorGUILayout.Popup("A/X", AXKey.intValue, input.VRActions);
					AXKeyOculus.intValue = AXKey.intValue;
				}
			}

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.HelpBox("The VRInput script allows you to specify a list of custom actions. " +
				"Do this by expanding the 'Edit Actions' foldout and adding or removing from the list, " +
				"You can then assign the actions to controller keys.\n" +
				"The method 'InputReceived' is called on this object using a SendMessage call. You can implement this " +
				"method in any script on this object.", MessageType.Info);
		}

		public void ResetToInteractbaleDefault()
		{
			input.VRActions = new string[] { "NONE", "ACTION", "PICKUP_DROP" };

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

			EditorUtility.SetDirty(input);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}

}
