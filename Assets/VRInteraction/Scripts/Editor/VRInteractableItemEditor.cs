//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Editor for VRInteractableItem. Allows for local held position and rotation
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRInteraction
{

	[CustomEditor(typeof(VRInteractableItem))]
	public class VRInteractableItemEditor : Editor 
	{
		public VRInteractableItem interactableItem = null;

		virtual public void OnEnable()
		{
			interactableItem = (VRInteractableItem)target;
		}

		public override void OnInspectorGUI()
		{
			ItemSection(serializedObject);
		}

		static public bool ItemSection(SerializedObject interactableItem, bool showPickupVars = true, bool requireParentItem = false, bool updateAndApply = true)
		{
			if (updateAndApply) interactableItem.Update();

			bool changed = false;

			SerializedProperty item = interactableItem.FindProperty("item");
			var oldItem = item.objectReferenceValue;
			item.objectReferenceValue = EditorGUILayout.ObjectField("Root Object", item.objectReferenceValue, typeof(Transform), true);
			if (item.objectReferenceValue != oldItem) changed = true;
			if (item.objectReferenceValue == null)
			{
				EditorGUILayout.HelpBox("Item requires a root object (this can be itself)", MessageType.Warning);
			} else
			{
				SerializedProperty parents = interactableItem.FindProperty("parents");
				var oldParents = parents;
				GUIContent parentItemContent = new GUIContent("Parent Item", "A parent item means this object cannot be interacted with unless the parent is being held. E.g. the slide of a gun");
				EditorGUILayout.PropertyField(parents, parentItemContent, true);
				if (parents != oldParents) changed = true;

				if (requireParentItem && parents.arraySize == 0)
				{
					EditorGUILayout.HelpBox("This item requires a parent item in order to work", MessageType.Error);
				}

				SerializedProperty canBeHeld = interactableItem.FindProperty("canBeHeld");
				if (showPickupVars)
				{
					SerializedProperty holdType = interactableItem.FindProperty("holdType");
					var oldJointHold = holdType.enumValueIndex;
					holdType.enumValueIndex = (int)(VRInteractableItem.HoldType)EditorGUILayout.EnumPopup("Hold Type", (VRInteractableItem.HoldType)holdType.enumValueIndex);
					if (oldJointHold != holdType.enumValueIndex) changed = true;
					if (GUILayout.Button("Setup Held Position"))
					{
						HeldPositionWindow newWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Held Position", true);
						newWindow.interactableItem = (VRInteractableItem)interactableItem.targetObject;
						newWindow.Init();
					}

					SerializedProperty followForce = interactableItem.FindProperty("followForce");
					var oldFollowForce = followForce.floatValue;
					EditorGUILayout.PropertyField(followForce);
					if (followForce.floatValue != oldFollowForce) changed = true;

					SerializedProperty throwBoost = interactableItem.FindProperty("throwBoost");
					var oldThrowBoost = throwBoost.floatValue;
					throwBoost.floatValue = EditorGUILayout.FloatField("Throw Boost", throwBoost.floatValue);
					if (throwBoost.floatValue != oldThrowBoost) changed = true;

					var oldCanBeHeld = canBeHeld.boolValue;
					EditorGUILayout.PropertyField(canBeHeld);
					if (canBeHeld.boolValue != oldCanBeHeld) changed = true;
				} else
				{
					var oldCanBeHeld = canBeHeld.boolValue;
					canBeHeld.boolValue = false;
					if (canBeHeld.boolValue != oldCanBeHeld) changed = true;

					if (GUILayout.Button("Setup Held Position"))
					{
						HeldPositionWindow newWindow = (HeldPositionWindow)EditorWindow.GetWindow(typeof(HeldPositionWindow), true, "Held Position", true);
						newWindow.interactableItem = (VRInteractableItem)interactableItem.targetObject;
						newWindow.Init();
					}
				}

				SerializedProperty toggleToPickup = interactableItem.FindProperty("toggleToPickup");
				var oldToggleToPickup = toggleToPickup.boolValue;
				toggleToPickup.boolValue = EditorGUILayout.Toggle("Toggle To Pickup", toggleToPickup.boolValue);
				if (toggleToPickup.boolValue != oldToggleToPickup) changed = true;

				SerializedProperty useBreakDistance = interactableItem.FindProperty("useBreakDistance");
				string breakDistanceTooltip = "If the distance between the controller and held position is greater than " +
					"the break distance it will drop the item. Useful for items that cannot be held.";
				GUIContent useBreakDistanceContent = new GUIContent("Use Break Distance", breakDistanceTooltip);
				EditorGUILayout.PropertyField(useBreakDistance, useBreakDistanceContent);
				if (useBreakDistance.boolValue)
				{
					SerializedProperty breakDistance = interactableItem.FindProperty("breakDistance");
					GUIContent breakDistanceContent = new GUIContent("Break Distance", breakDistanceTooltip);
					EditorGUILayout.PropertyField(breakDistance, breakDistanceContent);
				}

				SerializedProperty interactionDistance = interactableItem.FindProperty("interactionDistance");
				var oldInteractionDistance = interactionDistance.floatValue;
				EditorGUILayout.PropertyField(interactionDistance);
				if (interactionDistance.floatValue != oldInteractionDistance) changed = true;

				SerializedProperty interactionDisabled = interactableItem.FindProperty("interactionDisabled");
				var oldInteractionDisabled = interactionDisabled.boolValue;
				EditorGUILayout.PropertyField(interactionDisabled);
				if (interactionDisabled.boolValue != oldInteractionDisabled) changed = true;

				SerializedProperty limitAcceptedAction = interactableItem.FindProperty("limitAcceptedAction");
				var oldLimitAcceptedActions = limitAcceptedAction.boolValue;
				EditorGUILayout.PropertyField(limitAcceptedAction);
				if (limitAcceptedAction.boolValue != oldLimitAcceptedActions) changed = true;

				if (limitAcceptedAction.boolValue)
				{
					SerializedProperty acceptedActions = interactableItem.FindProperty("acceptedActions");
					var oldAcceptedAction = acceptedActions;
					EditorGUILayout.PropertyField(acceptedActions, true);
					if (acceptedActions != oldAcceptedAction) changed = true;
				}

				SerializedProperty itemId = interactableItem.FindProperty("itemId");
				GUIContent itemIdContent = new GUIContent("Item ID", "Used with inventory");// and for bullets and attachments if 'use string id' is toggled on");
				EditorGUILayout.PropertyField(itemId, itemIdContent);

				VRInteractableItemEditor.DisplayHoverSegment(interactableItem);

				VRInteractableItemEditor.DisplayTriggerColliderSegment(interactableItem);

				VRInteractableItemEditor.DisplaySoundSegment(interactableItem);

				VRInteractableItemEditor.DisplayIKSegment(interactableItem);

				SerializedProperty pickupEvent = interactableItem.FindProperty("pickupEvent");
				var oldPickupEvent = pickupEvent;
				SerializedProperty dropEvent = interactableItem.FindProperty("dropEvent");
				var oldDropEvent = dropEvent;
				if (canBeHeld.boolValue)
				{
					EditorGUILayout.PropertyField(pickupEvent);
					if (pickupEvent != oldPickupEvent) changed = true;
					EditorGUILayout.PropertyField(dropEvent);
					if (dropEvent != oldDropEvent) changed = true;
				} else
				{
					GUIContent pickupContent = new GUIContent("Clicked Event");	
					EditorGUILayout.PropertyField(pickupEvent, pickupContent);
					if (pickupEvent != oldPickupEvent) changed = true;
				}
				SerializedProperty enableHoverEvent = interactableItem.FindProperty("enableHoverEvent");
				EditorGUILayout.PropertyField(enableHoverEvent);
				SerializedProperty disableHoverEvent = interactableItem.FindProperty("disableHoverEvent");
				EditorGUILayout.PropertyField(disableHoverEvent);
			}

			if (updateAndApply) interactableItem.ApplyModifiedProperties();

			return changed;
		}

		public static bool DisplayHoverSegment(SerializedObject interactableItem)
		{
			bool changed = false;

			SerializedProperty hovers = interactableItem.FindProperty("hovers");
			SerializedProperty defaultShaders = interactableItem.FindProperty("defaultShaders");
			SerializedProperty hoverShaders = interactableItem.FindProperty("hoverShaders");
			SerializedProperty defaultMats = interactableItem.FindProperty("defaultMats");
			SerializedProperty hoverMats = interactableItem.FindProperty("hoverMats");
			SerializedProperty hoverModes = interactableItem.FindProperty("hoverModes");
			var oldHoverModeArraySize = hoverModes.arraySize;
			var olddefaultShadersArraySize = hoverModes.arraySize;
			var oldhoverShadersArraySize = hoverModes.arraySize;
			var olddefaultMatsArraySize = hoverModes.arraySize;
			var oldhoverMatsArraySize = hoverModes.arraySize;
			hoverModes.arraySize = defaultShaders.arraySize = hoverShaders.arraySize = defaultMats.arraySize = hoverMats.arraySize = hovers.arraySize;
			if (oldHoverModeArraySize != hoverModes.arraySize ||
				olddefaultShadersArraySize != defaultShaders.arraySize ||
				oldhoverShadersArraySize != hoverShaders.arraySize ||
				olddefaultMatsArraySize != defaultMats.arraySize ||
				oldhoverMatsArraySize != hoverMats.arraySize) changed = true;

			SerializedProperty hoverFoldout = interactableItem.FindProperty("hoverFoldout");
			hoverFoldout.boolValue = EditorGUILayout.Foldout(hoverFoldout.boolValue, "Hovers");
			if (!hoverFoldout.boolValue) return changed;

			EditorGUILayout.HelpBox("All intractable items will have a hover section, here you can choose exactly how you want it " +
				"to work, by default it will use an unlit shader for the hover. You " +
				"can chose a different shader or you can switch to two materials. " +
				"Leaving the default shader blank will have it use the shader or " +
				"material the mesh renderer has at startup.", MessageType.Info);

			var oldArraySize = hovers.arraySize;
			hoverModes.arraySize = defaultShaders.arraySize = hoverShaders.arraySize = defaultMats.arraySize = hoverMats.arraySize = hovers.arraySize = EditorGUILayout.IntField("Size", hovers.arraySize);
			if (oldArraySize != hovers.arraySize) changed = true;
			for(int i=0; i<hovers.arraySize; i++)
			{
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				SerializedProperty hover = hovers.GetArrayElementAtIndex(i);
				var oldHover = hover.objectReferenceValue;
				hover.objectReferenceValue = EditorGUILayout.ObjectField("Hover", hover.objectReferenceValue, typeof(Renderer), true);
				if (oldHover != hover.objectReferenceValue) changed = true;
				if (hover.objectReferenceValue != null)
				{
					SerializedProperty hoverMode = hoverModes.GetArrayElementAtIndex(i);
					var oldHoverMode = hoverMode.intValue;
					hoverMode.intValue = (int)(VRInteractableItem.HoverMode)EditorGUILayout.EnumPopup("Hover Mode", (VRInteractableItem.HoverMode)hoverMode.intValue);
					if (oldHoverMode != hoverMode.intValue) changed = true;
					VRInteractableItem.HoverMode hoverModeEnum = (VRInteractableItem.HoverMode)hoverMode.intValue;
					switch(hoverModeEnum)
					{
					case VRInteractableItem.HoverMode.SHADER:
						EditorGUILayout.HelpBox("Leave null to use current materials shader", MessageType.Info);
						SerializedProperty defaultShader = defaultShaders.GetArrayElementAtIndex(i);
						var oldDefaultShader = defaultShader.objectReferenceValue;
						defaultShader.objectReferenceValue = EditorGUILayout.ObjectField("Default Shader", defaultShader.objectReferenceValue, typeof(Shader), false);
						if (oldDefaultShader != defaultShader.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverShader = hoverShaders.GetArrayElementAtIndex(i);
						var oldHoverShader = hoverShader.objectReferenceValue;
						hoverShader.objectReferenceValue = EditorGUILayout.ObjectField("Hover Shader", hoverShader.objectReferenceValue, typeof(Shader), false);
						if (oldHoverShader != hoverShader.objectReferenceValue) changed = true;
						break;
					case VRInteractableItem.HoverMode.MATERIAL:
						EditorGUILayout.HelpBox("Leave null to use current material", MessageType.Info);
						SerializedProperty defaultMat = defaultMats.GetArrayElementAtIndex(i);
						var oldDefaultMat = defaultMat.objectReferenceValue;
						defaultMat.objectReferenceValue = EditorGUILayout.ObjectField("Default Material", defaultMat.objectReferenceValue, typeof(Material), false);
						if (oldDefaultMat != defaultMat.objectReferenceValue) changed = true;
						EditorGUILayout.HelpBox("Hover Default is Unlit/Texture", MessageType.Info);
						SerializedProperty hoverMat = hoverMats.GetArrayElementAtIndex(i);
						var oldHoverMat = hoverMat.objectReferenceValue;
						hoverMat.objectReferenceValue = EditorGUILayout.ObjectField("Hover Material", hoverMat.objectReferenceValue, typeof(Material), false);
						if (oldHoverMat != hoverMat.objectReferenceValue) changed = true;
						break;
					}
				}
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			return changed;
		}

		public static bool DisplayTriggerColliderSegment(SerializedObject interactableItem)
		{
			bool changed = false;
			SerializedProperty triggersFoldout = interactableItem.FindProperty("triggersFoldout");
			triggersFoldout.boolValue = EditorGUILayout.Foldout(triggersFoldout.boolValue, "Interaction Colliders");
			if (!triggersFoldout.boolValue) return changed;

			EditorGUILayout.HelpBox("You can assign colliders here for larger items, normally you can just use the 'Interaction Distance' " +
				"variable, which looks at the distance between the controller and the item pivot point. However for larger more complext objects " +
				"you can reference colliders here, if the controller is within the bounds of these colliders it will hover if there are no closer " +
				"items.", MessageType.Info);

			SerializedProperty triggerColliders = interactableItem.FindProperty("triggerColliders");
			var oldArraySize = triggerColliders.arraySize;
			triggerColliders.arraySize = EditorGUILayout.IntField("Size", triggerColliders.arraySize);
			if (oldArraySize != triggerColliders.arraySize) changed = true;
			for(int i=0; i<triggerColliders.arraySize; i++)
			{
				SerializedProperty triggerCollider = triggerColliders.GetArrayElementAtIndex(i);
				var oldTriggerCollider = triggerCollider.objectReferenceValue;
				triggerCollider.objectReferenceValue = EditorGUILayout.ObjectField("Collider", triggerCollider.objectReferenceValue, typeof(Collider), true);
				if (oldTriggerCollider != triggerCollider.objectReferenceValue) changed = true;
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			return changed;
		}

		public static void DisplaySoundSegment(SerializedObject interactableItem)
		{
			SerializedProperty soundsFoldout = interactableItem.FindProperty("soundsFoldout");
			soundsFoldout.boolValue = EditorGUILayout.Foldout(soundsFoldout.boolValue, "Sounds");
			if (!soundsFoldout.boolValue) return;

			SerializedProperty audioSource = interactableItem.FindProperty("audioSource");
			GUIContent audioSourceContent = new GUIContent("Audio Source", "Optionally specify and audio source, if left null will use AudioSource.PlayClipAtPoint");
			EditorGUILayout.PropertyField(audioSource, audioSourceContent);
			SerializedProperty enterHover = interactableItem.FindProperty("enterHover");
			EditorGUILayout.PropertyField(enterHover);
			SerializedProperty exitHover = interactableItem.FindProperty("exitHover");
			EditorGUILayout.PropertyField(exitHover);
			SerializedProperty pickupSound = interactableItem.FindProperty("pickupSound");
			EditorGUILayout.PropertyField(pickupSound);
			SerializedProperty dropSound = interactableItem.FindProperty("dropSound");
			EditorGUILayout.PropertyField(dropSound);
			SerializedProperty forceGrabSound = interactableItem.FindProperty("forceGrabSound");
			EditorGUILayout.PropertyField(forceGrabSound);
		}

		public static void DisplayIKSegment(SerializedObject interactableItem)
		{
			SerializedProperty ikFoldout = interactableItem.FindProperty("ikFoldout");
			ikFoldout.boolValue = EditorGUILayout.Foldout(ikFoldout.boolValue, "IK");
			if (!ikFoldout.boolValue) return;

			string handAnchorTooltip = "If using an IK rig this is the transform the target will be" +
				" moved to when picked up, this transform should be an empty" +
				" object as a child of item and used for lining up the models held position";
			string handPoseTooltip = "Use the HandPoseController and setup hand poses, then list the " +
				"name of the pose object here. Check the FinalIK integration folder and prefabs for reference.";

			SerializedProperty leftHandAnchor = interactableItem.FindProperty("leftHandIKAnchor");
			GUIContent leftHandAnchorContent = new GUIContent("Left Hand IK Anchor", handAnchorTooltip);
			EditorGUILayout.PropertyField(leftHandAnchor, leftHandAnchorContent);

			SerializedProperty leftHandIKPoseName = interactableItem.FindProperty("leftHandIKPoseName");
			GUIContent leftHandPoseContent = new GUIContent("Left Hand IK Pose Name", handAnchorTooltip);
			EditorGUILayout.PropertyField(leftHandIKPoseName, leftHandPoseContent);

			SerializedProperty rightHandAnchor = interactableItem.FindProperty("rightHandIKAnchor");
			GUIContent rightHandAnchorContent = new GUIContent("Right Hand IK Anchor", handAnchorTooltip);
			EditorGUILayout.PropertyField(rightHandAnchor, rightHandAnchorContent);

			SerializedProperty rightHandIkPoseName = interactableItem.FindProperty("rightHandIkPoseName");
			GUIContent rightHandPoseContent = new GUIContent("Right Hand IK Pose Name", handPoseTooltip);
			EditorGUILayout.PropertyField(rightHandIkPoseName, rightHandPoseContent);
		}
	}
}
