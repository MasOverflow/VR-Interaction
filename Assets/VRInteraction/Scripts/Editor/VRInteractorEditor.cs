//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRInteractor. Allows you set set the interaction sphere size and
// Add a reference for an object that will be picked up on enable
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace VRInteraction
{
	[CustomEditor(typeof(VRInteractor))]
	public class VRInteractorEditor : Editor {

		// target component
		public VRInteractor interactor = null;

		bool badObjectReference = false;

		virtual public void OnEnable()
		{
			interactor = (VRInteractor)target;
			if (interactor.GetComponent<VRInput>() == null)
			{
				interactor.gameObject.AddComponent<VRInput>();
				EditorUtility.SetDirty(interactor);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty controllerAnchor = serializedObject.FindProperty("controllerAnchor");
			SerializedProperty controllerAnchorOffset = serializedObject.FindProperty("controllerAnchorOffset");
			GUIContent controllerAnchorContent = new GUIContent("Controller Anchor", "Reference the hand of a IK model if you are using one, otherwise leave blank to use the controller");
			if (controllerAnchor.objectReferenceValue == null)
			{
				controllerAnchor.objectReferenceValue = interactor.transform;
			}
			var oldControllerAnchor = controllerAnchor.objectReferenceValue;
			EditorGUILayout.PropertyField(controllerAnchor, controllerAnchorContent);
			if (oldControllerAnchor != controllerAnchor.objectReferenceValue)
			{
				if (controllerAnchorOffset.objectReferenceValue != null)
					DestroyImmediate(((Transform)controllerAnchorOffset.objectReferenceValue).gameObject);
				controllerAnchorOffset.objectReferenceValue = null;
			}

			if (controllerAnchorOffset.objectReferenceValue == null)
			{
				Transform newAnchorOffset = null;
				foreach(Transform trans in (Transform)controllerAnchor.objectReferenceValue)
				{
					if (trans.name == VRInteractor.anchorOffsetName)
					{
						newAnchorOffset = trans;
						break;
					}
				}
				if (newAnchorOffset == null) interactor.controllerAnchor = (Transform)controllerAnchor.objectReferenceValue;
				controllerAnchorOffset.objectReferenceValue = newAnchorOffset == null ? interactor.getControllerAnchorOffset : newAnchorOffset;
			}
			EditorGUILayout.PropertyField(controllerAnchorOffset);

			SerializedProperty ikTarget = serializedObject.FindProperty("ikTarget");
			GUIContent ikTargetContent = new GUIContent("IK Target", "If using an IK rig this should be the hand target the rig is pointing to." +
														" Either this object or a child transform");
			EditorGUILayout.PropertyField(ikTarget, ikTargetContent);

			SerializedProperty referenceObject = serializedObject.FindProperty("objectReference");
			GUIContent referenceContent = new GUIContent("Object Reference", "The object reference will be set to held at the start. You can use this if you want to fix a gun to the player that can't be dropped");
			referenceObject.objectReferenceValue = EditorGUILayout.ObjectField(referenceContent, referenceObject.objectReferenceValue, typeof(GameObject), true);
			if (badObjectReference) EditorGUILayout.HelpBox("Object reference must be an instance in the scene and must have a VRInteractableItem (or something that inherits) script attached", MessageType.Warning);
			if (referenceObject.objectReferenceValue != null)
			{
				SerializedProperty objectReferenceIsPrefab = serializedObject.FindProperty("objectReferenceIsPrefab");
				badObjectReference = false;
				PrefabType objectPrefabType = PrefabUtility.GetPrefabType(referenceObject.objectReferenceValue);
				VRInteractableItem interactableItem = ((GameObject)referenceObject.objectReferenceValue).GetComponentInChildren<VRInteractableItem>();
				if (interactableItem == null)
				{
					badObjectReference = true;
					referenceObject.objectReferenceValue = null;
				}
				objectReferenceIsPrefab.boolValue = objectPrefabType == PrefabType.ModelPrefab || objectPrefabType == PrefabType.Prefab;
			}

			SerializedProperty useHoverLine = serializedObject.FindProperty("useHoverLine");
			EditorGUILayout.PropertyField(useHoverLine);
			if (useHoverLine.boolValue)
			{
				SerializedProperty hoverLineMat = serializedObject.FindProperty("hoverLineMat");
				EditorGUILayout.PropertyField(hoverLineMat);
			}

			SerializedProperty hideControllersWhileHolding = serializedObject.FindProperty("hideControllersWhileHolding");
			GUIContent hideControllerContent = new GUIContent("Held Hide Controller", "Hide Controllers While Holding Item");
			hideControllersWhileHolding.boolValue = EditorGUILayout.Toggle(hideControllerContent, hideControllersWhileHolding.boolValue);

			SerializedProperty forceGrabDirection = serializedObject.FindProperty("forceGrabDirection");
			GUIContent forceGrabDirectionContent = new GUIContent("Force Grab Direction", "Local Controller Direction, use (1,0,0) for palm or (0,0,1) for forward");
			EditorGUILayout.PropertyField(forceGrabDirection, forceGrabDirectionContent);

			SerializedProperty forceGrabDistance = serializedObject.FindProperty("forceGrabDistance");
			EditorGUILayout.PropertyField(forceGrabDistance);

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.HelpBox(
				"The VRInteractor script implements the 'Input Received' method called by VRInput, " +
				"it then calls the method matching the action name to any hovered or held VRInteractableItem.\n" +
				"By default an item can respond to:\n" + 
				"PICKUP_DROP: Can Pickup a hovered item or drop the currently held item\n" +
				"PICKUP: Can Pickup a hovered item\n" +
				"DROP: Drops the currently held item\n" +
				"ACTION: Calls PICKUP if no item is held (The BasicToggle script uses this to toggle the light on and off)" , MessageType.Info);
		}
	}
}
