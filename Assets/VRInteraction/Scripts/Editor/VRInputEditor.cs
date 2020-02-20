//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRInput
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
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

		static bool editActionsFoldout;
		string newActionName = "";

        public virtual void OnEnable()
        {
            input = (VRInput)target;
#if !Int_SteamVR2
            input.trackInputNatively = true;
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

            SerializedProperty leftHand = serializedObject.FindProperty("LeftHand");
            EditorGUILayout.PropertyField(leftHand);

#if UNITY_2017_2_OR_NEWER
            SerializedProperty trackPosNative = serializedObject.FindProperty("trackPositionNatively");
            EditorGUILayout.PropertyField(trackPosNative);
#if Int_SteamVR2
            SerializedProperty trackInputNative = serializedObject.FindProperty("trackInputNatively");
            EditorGUILayout.PropertyField(trackInputNative);
#endif
#endif


            if (input.trackInputNatively)
            {
                editActionsFoldout = EditorGUILayout.Foldout(editActionsFoldout, "Edit Actions");

                if (editActionsFoldout)
                {
                    if (input.VRActions != null)
                    {
                        for (int i = 0; i < input.VRActions.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            input.VRActions[i] = EditorGUILayout.TextField(input.VRActions[i]);
                            if (GUILayout.Button("X"))
                            {
                                string[] newActions = new string[input.VRActions.Length - 1];
                                int offset = 0;
                                for (int j = 0; j < newActions.Length; j++)
                                {
                                    if (i == j) offset = 1;
                                    newActions[j] = input.VRActions[j + offset];
                                }
                                input.VRActions = newActions;

                                if (input.triggerKey > i)
                                    input.triggerKey -= 1;
                                else if (input.triggerKey == i)
                                    input.triggerKey = 0;
                                for (int j = 0; j < input.triggerKeys.Count; j++)
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

                                if (input.primaryKey > i)
                                    input.primaryKey -= 1;
                                else if (input.primaryKey == i)
                                    input.primaryKey = 0;

                                for (int j = 0; j < input.primaryKeys.Count; j++)
                                {
                                    if (input.primaryKeys[j] > i)
                                        input.primaryKeys[j] -= 1;
                                    else if (input.primaryKeys[j] == i)
                                        input.primaryKeys[j] = 0;
                                }

                                if (input.secondaryKey > i)
                                    input.secondaryKey -= 1;
                                else if (input.secondaryKey == i)
                                    input.secondaryKey = 0;

                                for (int j = 0; j < input.secondaryKeys.Count; j++)
                                {
                                    if (input.secondaryKeys[j] > i)
                                        input.secondaryKeys[j] -= 1;
                                    else if (input.secondaryKeys[j] == i)
                                        input.secondaryKeys[j] = 0;
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
                        if (input.VRActions != null) newActions = new string[input.VRActions.Length + 1];
                        else input.VRActions = new string[0];
                        for (int i = 0; i < newActions.Length; i++)
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
                SerializedProperty primaryKey = serializedObject.FindProperty("primaryKey");
                SerializedProperty secondaryKey = serializedObject.FindProperty("secondaryKey");

                SerializedProperty triggerKeys = serializedObject.FindProperty("triggerKeys");
                SerializedProperty padTops = serializedObject.FindProperty("padTops");
                SerializedProperty padLefts = serializedObject.FindProperty("padLefts");
                SerializedProperty padRights = serializedObject.FindProperty("padRights");
                SerializedProperty padBottoms = serializedObject.FindProperty("padBottoms");
                SerializedProperty padCentres = serializedObject.FindProperty("padCentres");
                SerializedProperty padTouchs = serializedObject.FindProperty("padTouchs");
                SerializedProperty gripKeys = serializedObject.FindProperty("gripKeys");
                SerializedProperty menuKeys = serializedObject.FindProperty("menuKeys");
                SerializedProperty primaryKeys = serializedObject.FindProperty("primaryKeys");
                SerializedProperty secondaryKeys = serializedObject.FindProperty("secondaryKeys");


                triggerKey.intValue = EditorGUILayout.Popup("Trigger Key", triggerKey.intValue, input.VRActions);
                AdditionalButtonArray(triggerKeys, "Trigger Key");
                padTop.intValue = EditorGUILayout.Popup("Thumbstick Up", padTop.intValue, input.VRActions);

                AdditionalButtonArray(padTops, "Thumbstick Up");

                padLeft.intValue = EditorGUILayout.Popup("Thumbstick Left", padLeft.intValue, input.VRActions);

                AdditionalButtonArray(padLefts, "Thumbstick Left");

                padRight.intValue = EditorGUILayout.Popup("Thumbstick Right", padRight.intValue, input.VRActions);

                AdditionalButtonArray(padRights, "Thumbstick Right");

                padBottom.intValue = EditorGUILayout.Popup("Thumbstick Down", padBottom.intValue, input.VRActions);

                AdditionalButtonArray(padBottoms, "Thumbstick Down");

                padCentre.intValue = EditorGUILayout.Popup("Thumbstick Button", padCentre.intValue, input.VRActions);

                AdditionalButtonArray(padCentres, "Thumbstick Button");

                padTouch.intValue = EditorGUILayout.Popup("Thumbstick Touch", padTouch.intValue, input.VRActions);

                AdditionalButtonArray(padTouchs, "Thumbstick Touch");

                gripKey.intValue = EditorGUILayout.Popup("Grip Key", gripKey.intValue, input.VRActions);

                AdditionalButtonArray(gripKeys, "Grip Key");

                menuKey.intValue = EditorGUILayout.Popup("Menu Key", menuKey.intValue, input.VRActions);

                AdditionalButtonArray(menuKeys, "Menu Key");

                primaryKey.intValue = EditorGUILayout.Popup("Primary", primaryKey.intValue, input.VRActions);
                AdditionalButtonArray(primaryKeys, "Primary");

                secondaryKey.intValue = EditorGUILayout.Popup("Secondary", secondaryKey.intValue, input.VRActions);
                AdditionalButtonArray(secondaryKeys, "Secondary");

                EditorGUILayout.HelpBox("The VRInput script allows you to specify a list of custom actions. " +
                "Do this by expanding the 'Edit Actions' foldout and adding or removing from the list, " +
                "You can then assign the actions to controller keys.\n" +
                "The method 'InputReceived' is called on this object using a SendMessage call. You can implement this " +
                "method in any script on this object.", MessageType.Info);

            }
            else
            {
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
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AdditionalButtonArray(SerializedProperty actionKeys, string displayName)
        {
            for (int x = 0; x < actionKeys.arraySize; x++)
            {
                SerializedProperty property = actionKeys.GetArrayElementAtIndex(x);
                int oldProperty = property.intValue;
                GUILayout.BeginHorizontal();
                property.intValue = EditorGUILayout.Popup(displayName, property.intValue, input.VRActions);

                if (GUILayout.Button("-"))
                {
                    actionKeys.DeleteArrayElementAtIndex(x);
                    break;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                actionKeys.arraySize++;
            }
            GUILayout.EndHorizontal();
        }

        public void ResetToInteractbaleDefault()
        {
            input.VRActions = new string[] { "NONE", "ACTION", "PICKUP_DROP" };

			input.triggerKey = 1;
			input.padTop = 0;
			input.padLeft = 0;
			input.padRight = 0;
			input.padBottom = 0;
			input.padCentre = 0;
			input.padTouch = 0;
			input.gripKey = 2;
			input.menuKey = 0;
			input.primaryKey = 0;
            input.secondaryKey = 0;

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
