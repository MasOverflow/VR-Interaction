//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Window used to set positions for InteractableItems
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VRInteraction
{
	public delegate void MagazineUpdateEventHandler(HeldPositionWindow sender);

	public class HeldPositionWindow : EditorWindow 
	{
		public VRInteractableItem interactableItem;
		public SerializedObject serializedItem;

		//VRWeaponHandler variables.
		//For Applying magazine held position.
		public event MagazineUpdateEventHandler OnSaveEvent;
		public bool gunHandlerWindow;
		public GameObject attachmentInstance;
		public GameObject attachmentPrefab;

		static GameObject controllerInstance;
		bool leftHand = true;
		Vector3 oldPosition;
		Quaternion oldRotation;
		Transform oldParent;

		public void Init()
		{
			if (interactableItem == null) return;
			if (serializedItem == null)
				serializedItem = new SerializedObject(interactableItem);
		}

		void OnGUI()
		{
			var oldInteractableItem = interactableItem;
			interactableItem = (VRInteractableItem)EditorGUILayout.ObjectField("Interactive Item", interactableItem, typeof(VRInteractableItem), true);
			if (interactableItem == null) return;
			if (oldInteractableItem != interactableItem || serializedItem == null)
				Init();

			serializedItem.Update();

			SerializedProperty item = serializedItem.FindProperty("item");
			if (item.objectReferenceValue == null)
			{
				item.objectReferenceValue = EditorGUILayout.ObjectField("Item", item.objectReferenceValue, typeof(Transform), true);
				if (item.objectReferenceValue == null) return;
			}

			SerializedProperty linkedLeftAndRightHeldPositions = serializedItem.FindProperty("linkedLeftAndRightHeldPositions");
			linkedLeftAndRightHeldPositions.boolValue = EditorGUILayout.Toggle("Linked Left And Right Held Positions", linkedLeftAndRightHeldPositions.boolValue);



			SerializedProperty heldPosition = serializedItem.FindProperty("heldPosition");
			heldPosition.vector3Value = EditorGUILayout.Vector3Field(linkedLeftAndRightHeldPositions.boolValue ? "Held Position" : "Left Held Position", heldPosition.vector3Value);

			SerializedProperty heldRotation = serializedItem.FindProperty("heldRotation");
			Quaternion tempHeldRotation = heldRotation.quaternionValue;
			tempHeldRotation.eulerAngles = EditorGUILayout.Vector3Field(linkedLeftAndRightHeldPositions.boolValue ? "Held Rotation" : "Left Held Rotation", tempHeldRotation.eulerAngles);
			heldRotation.quaternionValue = tempHeldRotation;

			SerializedProperty heldPositionRight = serializedItem.FindProperty("heldPositionRightHand");
			SerializedProperty heldRotationRightHand = serializedItem.FindProperty("heldRotationRightHand");
			if (!linkedLeftAndRightHeldPositions.boolValue)
			{
				heldPositionRight.vector3Value = EditorGUILayout.Vector3Field("Right Held Position", heldPositionRight.vector3Value);

				Quaternion tempHeldRotationRightHand = heldRotationRightHand.quaternionValue;
				tempHeldRotationRightHand.eulerAngles = EditorGUILayout.Vector3Field("Right Held Rotation", tempHeldRotationRightHand.eulerAngles);
				heldRotationRightHand.quaternionValue = tempHeldRotationRightHand;
			}

			/*SerializedProperty heldPositionOculus = serializedItem.FindProperty("heldPositionOculus");
			heldPositionOculus.vector3Value = EditorGUILayout.Vector3Field(linkedLeftAndRightHeldPositions.boolValue ? "Held Position Oculus" : "Left Held Position Oculus", heldPositionOculus.vector3Value);
			SerializedProperty heldRotationOculus = serializedItem.FindProperty("heldRotationOculus");
			Quaternion tempHeldRotationOculus = heldRotationOculus.quaternionValue;
			tempHeldRotationOculus.eulerAngles = EditorGUILayout.Vector3Field(linkedLeftAndRightHeldPositions.boolValue ? "Held Rotation Oculus" : "Left Held Rotation Oculus", tempHeldRotationOculus.eulerAngles);
			heldRotationOculus.quaternionValue = tempHeldRotationOculus;

			SerializedProperty heldPositionOculusRightHand = serializedItem.FindProperty("heldPositionOculusRightHand");
			SerializedProperty heldRotationOculusRightHand = serializedItem.FindProperty("heldRotationOculusRightHand");
			if (!linkedLeftAndRightHeldPositions.boolValue)
			{
				heldPositionOculusRightHand.vector3Value = EditorGUILayout.Vector3Field("Right Held Position Oculus", heldPositionOculusRightHand.vector3Value);

				Quaternion tempHeldRotationOculusRightHand = heldRotationOculusRightHand.quaternionValue;
				tempHeldRotationOculusRightHand.eulerAngles = EditorGUILayout.Vector3Field("Right Held Rotation Oculus", tempHeldRotationOculusRightHand.eulerAngles);
				heldRotationOculusRightHand.quaternionValue = tempHeldRotationOculusRightHand;
			}*/

			bool updatePrefab = false;
			if (controllerInstance == null)
			{
				bool makingReferenceController = false;
				GameObject referenceControllerPrefab = null;
				if (GUILayout.Button("Create SteamVR Reference Controller"))
				{
					makingReferenceController = true;
					referenceControllerPrefab = Resources.Load<GameObject>("ViveController");
					if (referenceControllerPrefab == null) Debug.LogError("Can't find ViveController in resources");
				}
				if (GUILayout.Button("Create Oculus Reference Controller"))
				{
					makingReferenceController = true;
					referenceControllerPrefab = Resources.Load<GameObject>("OculusController");
					if (referenceControllerPrefab == null) Debug.LogError("Can't find OculusController in resources");
				}
				if (makingReferenceController && referenceControllerPrefab != null)
				{
					leftHand = true;
					controllerInstance = (GameObject)Instantiate(referenceControllerPrefab, Vector3.zero, Quaternion.identity);
					Undo.RegisterCreatedObjectUndo(controllerInstance, "Create Reference Controller");
					oldPosition = interactableItem.item.position;
					oldRotation = interactableItem.item.rotation;
					oldParent = interactableItem.item.parent;
					interactableItem.item.SetParent(controllerInstance.transform);
					interactableItem.item.localPosition = heldPosition.vector3Value;
					interactableItem.item.localRotation = heldRotation.quaternionValue;
					Vector3 diff = oldPosition - interactableItem.item.position;
					controllerInstance.transform.position = diff+heldPosition.vector3Value;
					controllerInstance.transform.rotation = oldRotation;
					Selection.activeGameObject = interactableItem.item.gameObject;
				}
			} else
			{
				EditorGUILayout.HelpBox("Make sure to move the target item and not just the object with this script on", MessageType.Info);
				bool saveChange = false;
				if (GUILayout.Button(linkedLeftAndRightHeldPositions.boolValue ? "Save" : "Save Left"))
				{
					saveChange = true;
					heldPosition.vector3Value = interactableItem.item.localPosition;
					heldRotation.quaternionValue = interactableItem.item.localRotation;

				}
				if (!linkedLeftAndRightHeldPositions.boolValue)
				{
					if (GUILayout.Button("Save Right"))
					{
						saveChange = true;
						heldPositionRight.vector3Value = interactableItem.item.localPosition;
						heldRotationRightHand.quaternionValue = interactableItem.item.localRotation;
					}
				}
				if (saveChange)
				{
					Undo.SetTransformParent(interactableItem.item, oldParent, "Save Changes");
					Undo.RecordObject(interactableItem.item, "Save Changes");
					interactableItem.item.position = oldPosition;
					interactableItem.item.rotation = oldRotation;
					Undo.DestroyObjectImmediate(controllerInstance);
					if (attachmentInstance != null && gunHandlerWindow)
						updatePrefab = true;
				}

				if (!linkedLeftAndRightHeldPositions.boolValue)
				{
					if (GUILayout.Button("Toggle " + (leftHand ? "Right" : "Left") + " Hand Position"))
					{
						leftHand = !leftHand;
						if (leftHand)
						{
							interactableItem.item.localPosition = heldPosition.vector3Value;
							interactableItem.item.localRotation = heldRotation.quaternionValue;
						} else
						{
							interactableItem.item.localPosition = heldPositionRight.vector3Value;
							interactableItem.item.localRotation = heldRotationRightHand.quaternionValue;
						}
					}
				}

				if (GUILayout.Button("Select Controller"))
				{
					Selection.activeGameObject = controllerInstance;
				}
				if (GUILayout.Button("Select Item"))
				{
					Selection.activeGameObject = interactableItem.item.gameObject;
				}
				if (GUILayout.Button("Cancel"))
				{
					Undo.SetTransformParent(interactableItem.item, oldParent, "Cancel");
					Undo.RecordObject(interactableItem.item, "Save Changes");
					interactableItem.item.position = oldPosition;
					interactableItem.item.rotation = oldRotation;
					Undo.DestroyObjectImmediate(controllerInstance);
				}
			}
			serializedItem.ApplyModifiedProperties();

			EditorGUILayout.HelpBox("You can set up the held position using either the SteamVR Vive Controller or" +
				" the Native Oculus Controller. The two systems use a different base rotation so you can either base" +
				" the held position on steamVR or Oculus, so choose which one then set up all of your held positions with" +
				" that controller. For the other controller you can use the 'Controller Rotation Offset' value on the interactor" +
				" script attached to each controller to offset for either the alternative controller or to offset an alternative " +
				"anchor in the case of an IK hand for when you're not using the controller as the anchor.", MessageType.Info);


			if (updatePrefab && attachmentInstance != null && gunHandlerWindow && OnSaveEvent != null)
				OnSaveEvent(this);

		}

		void OnDestroy()
		{
			if (interactableItem == null || controllerInstance == null) return;
			Undo.SetTransformParent(interactableItem.item, oldParent, "Close");
			Undo.RecordObject(interactableItem.item, "Close");
			interactableItem.item.position = oldPosition;
			interactableItem.item.rotation = oldRotation;
			Undo.DestroyObjectImmediate(controllerInstance);
		}
	}
}
