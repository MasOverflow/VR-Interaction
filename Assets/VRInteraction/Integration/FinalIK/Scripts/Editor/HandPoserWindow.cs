//========= Copyright 2019, Sam Tague, All rights reserved. ===================
//
// Window used to set positions for InteractableItems
//
//===================Contact Email: Sam@MassGames.co.uk===========================


#if Int_FinalIK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using RootMotion.FinalIK;

namespace VRInteraction
{
	public class HandPoserWindow : EditorWindow 
	{
		protected enum Hand
		{
			LEFT,
			RIGHT
		}

		protected VRIK _vrIK;
		protected VRInteractableItem _item;
		protected HandPoseController _handPoseController;
		protected string _leftPoseName;
		protected string _rightPoseName;
		protected Hand _hand = Hand.LEFT;
		protected int _leftPoseIndex;
		protected int _rightPoseIndex;
		protected string[] _allPoses;

		[MenuItem("VR Weapon Interactor/FinalIK Hand Poser", false, 0)]
		public static void MenuInitHandPoser()
		{
			EditorWindow.GetWindow(typeof(HandPoserWindow), true, "Final IK Hand Poser", true);
		}

		virtual protected void OnGUI () 
		{
			GUILayout.Label ("Welcome To The FinalIK hand poser for VRInteraction items", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Reference the VRIK component of the target character you want to set hand poses for, then reference " +
				"the item you want the hand pose for.", MessageType.Info);
			var oldVRIK = _vrIK;
			_vrIK = (VRIK)EditorGUILayout.ObjectField("VRIK", _vrIK, typeof(VRIK), true);
			if (oldVRIK != null && _vrIK != oldVRIK) 
			{
				ResetHands(oldVRIK);
				_handPoseController = null;
			}
			_item = (VRInteractableItem)EditorGUILayout.ObjectField("VR Interactable Item", _item, typeof(VRInteractableItem), true);

			if (_vrIK == null || _item == null || _item.item == null || IsPrefab(_item.item.gameObject)) return;

			if (_handPoseController == null)
			{
				_handPoseController = _vrIK.GetComponent<HandPoseController>();
				if (_handPoseController == null) _handPoseController = _vrIK.gameObject.AddComponent<HandPoseController>();

				//Set pose name to default if set
				foreach(Transform pose in _handPoseController.poses)
				{
					if (pose.name == _item.leftHandIKPoseName) _leftPoseName = pose.name;
					if (pose.name == _item.rightHandIkPoseName) _rightPoseName = pose.name;
				}
			}

			GUILayout.BeginHorizontal();
			switch(_hand)
			{
			case Hand.LEFT:
				GUILayout.Box("Left Hand", GUILayout.ExpandWidth(true));
				if (GUILayout.Button("Right Hand"))
					_hand = Hand.RIGHT;
				break;
			case Hand.RIGHT:
				if (GUILayout.Button("Left Hand"))
					_hand = Hand.LEFT;
				GUILayout.Box("Right Hand", GUILayout.ExpandWidth(true));
				break;
			}
			GUILayout.EndHorizontal();

			Transform handTrans = _hand == Hand.LEFT ? _vrIK.references.leftHand : _vrIK.references.rightHand;
			if (handTrans == null)
			{
				EditorGUILayout.HelpBox("VRIK left or right hand reference is null", MessageType.Error);
				return;
			}

			DefaultPoses();

			bool changed = ShowExistingPoses();
			if (changed)
			{
				if (_hand == Hand.LEFT && _leftPoseIndex < _handPoseController.poses.Count &&
					_handPoseController.poses[_leftPoseIndex] != null) _leftPoseName = _handPoseController.poses[_leftPoseIndex].name;
				else if (_rightPoseIndex < _handPoseController.poses.Count &&
					_handPoseController.poses[_rightPoseIndex] != null) _rightPoseName = _handPoseController.poses[_rightPoseIndex].name;
			}

			if (_hand == Hand.LEFT) _leftPoseName = EditorGUILayout.TextField("Pose Name", _leftPoseName);
			else _rightPoseName = EditorGUILayout.TextField("Pose Name", _rightPoseName);
			if (GUILayout.Button("Move Hand To Item"))
			{
				MoveToItem();
			}
			if (GUILayout.Button("Reset Hand"))
			{
				ResetHands(_vrIK);
			}
			EditorGUI.BeginDisabledGroup((_hand == Hand.LEFT ? _vrIK.references.leftHand : _vrIK.references.rightHand) == null ||
				string.IsNullOrEmpty(_hand == Hand.LEFT ? _leftPoseName : _rightPoseName));
			if (GUILayout.Button("Select Hand Object"))
			{
				Selection.activeGameObject = _hand == Hand.LEFT ? _vrIK.references.leftHand.gameObject : _vrIK.references.rightHand.gameObject;
			}
			if (GUILayout.Button("Save Pose"))
			{
				SaveHand();
			}
			EditorGUI.EndDisabledGroup();
		}

		void OnDestroy()
		{
			ResetHands(_vrIK);
		}

		private bool ShowExistingPoses()
		{
			if (_allPoses == null || _allPoses.Length != _handPoseController.poses.Count)
			{
				_allPoses = new string[_handPoseController.poses.Count];
				for(int i=0;i<_handPoseController.poses.Count;i++)
				{
					_allPoses[i] = _handPoseController.poses[i].name;
				}
			}

			if (_hand == Hand.LEFT)
			{
				var oldPoseIndex = _leftPoseIndex;
				_leftPoseIndex = EditorGUILayout.Popup("Poses", _leftPoseIndex, _allPoses);
				if (oldPoseIndex != _leftPoseIndex) return true;
			} else
			{
				var oldPoseIndex = _rightPoseIndex;
				_rightPoseIndex = EditorGUILayout.Popup("Poses", _rightPoseIndex, _allPoses);
				if (oldPoseIndex != _rightPoseIndex) return true;
			}
			return false;
		}

		private void MoveToItem()
		{
			Transform handAnchor = _hand == Hand.LEFT ? _item.leftHandIKAnchor : _item.rightHandIKAnchor;
			Transform handTarget = _hand == Hand.LEFT ? _vrIK.references.leftHand : _vrIK.references.rightHand;
			if (handAnchor == null)
			{
				handTarget.position = _item.item.position;
				handTarget.rotation = _item.item.rotation;
			} else
			{
				handTarget.position = handAnchor.position;
				handTarget.rotation = handAnchor.rotation;
			}

			Transform poseReference = _handPoseController.GetPoseByName(_hand == Hand.LEFT ? _leftPoseName : _rightPoseName);
			if (poseReference == null) return;
			AssignPose(handTarget, poseReference);
		}

		virtual protected Transform SaveHand()
		{
			Transform handAnchor = _hand == Hand.LEFT ? _item.leftHandIKAnchor : _item.rightHandIKAnchor;
			Transform handTarget = _hand == Hand.LEFT ? _vrIK.references.leftHand : _vrIK.references.rightHand;

			//Update hand anchor to current hand position
			if (handAnchor == null)
			{
				GameObject handAnchorObj = new GameObject((_hand == Hand.LEFT ? "Left" : "Right") + "HandAnchor");
				handAnchor = handAnchorObj.transform;
				handAnchor.parent = _item.item;
				handAnchor.position = handTarget.position;
				handAnchor.rotation = handTarget.rotation;
				if (_hand == Hand.LEFT) _item.leftHandIKAnchor = handAnchor;
				else _item.rightHandIKAnchor = handAnchor;
			} else
			{
				handAnchor.position = handTarget.position;
				handAnchor.rotation = handTarget.rotation;
			}

			//Update item pose name
			if (_hand == Hand.LEFT) _item.leftHandIKPoseName = _leftPoseName;
			else _item.rightHandIkPoseName = _rightPoseName;

			//save item if prefab
			GameObject itemPrefab = ConvertToPrefabSource(_item.item.gameObject);
			if (itemPrefab != null)
			{
				GameObject rootInstance = _item.item.gameObject;
				if (_item.parents.Count != 0) rootInstance = _item.parents[0].item.gameObject;
				PrefabUtility.ReplacePrefab(rootInstance, itemPrefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
			}

			//Check hand pose controller list for pose name
			Transform foundPose = null;
			foreach(Transform pose in _handPoseController.poses)
			{
				if (pose.name != (_hand == Hand.LEFT ? _leftPoseName : _rightPoseName)) continue;
				foundPose = pose;
				break;
			}

			//	If found get rid of it
			if (foundPose != null)
			{
				_handPoseController.poses.Remove(foundPose);
				DestroyImmediate(foundPose.gameObject);
			}

			//	Create new pose and add to the list
			Transform newPoseRoot = CreateNewPoseObject(handTarget);
			newPoseRoot.name = _hand == Hand.LEFT ? _leftPoseName : _rightPoseName;
			_handPoseController.poses.Add(newPoseRoot);

			//Save player rig if prefab
			EditorUtility.SetDirty(_handPoseController);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

			return newPoseRoot;
		}

		protected bool IsPrefab(GameObject possiblePrefab)
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(possiblePrefab);
			return prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab;
		}

		private GameObject ConvertToPrefabSource(GameObject rootObject)
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(rootObject);
			if (prefabType == PrefabType.Prefab) return rootObject;
			else if (prefabType == PrefabType.PrefabInstance)
			{
				GameObject prefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(rootObject);
				return prefab;
			}
			return null;
			//return prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab;
		}

		//	Resets hands to saved default position and pose
		private void ResetHands(VRIK vrIK)
		{
			if (vrIK == null || _handPoseController == null) return;

			if (vrIK.references.leftHand != null && _handPoseController.defaultLeftPose != null) AssignPose(vrIK.references.leftHand, _handPoseController.defaultLeftPose, true);
			if (vrIK.references.rightHand != null && _handPoseController.defaultRightPose != null) AssignPose(vrIK.references.rightHand, _handPoseController.defaultRightPose, true);
		}

		private void AssignPose(Transform handTarget, Transform poseReference, bool moveRoot = false)
		{
			Transform[] handTargets = handTarget.GetComponentsInChildren<Transform>();
			Transform[] poseReferences = poseReference.GetComponentsInChildren<Transform>();
			for (int i = 0; i < handTargets.Length; i++) 
			{
				if (handTargets[i] == handTarget) //Is root hand object
				{
					if (moveRoot)
					{
						handTargets[i].position = poseReferences[i].position;
						handTargets[i].rotation = poseReferences[i].rotation;
					}
				} else
				{
					handTargets[i].localPosition = poseReferences[i].localPosition;
					handTargets[i].localRotation = poseReferences[i].localRotation;
				}
			}
		}

		//	Sets up default pose for hands so it can be reset later when window is closed
		private void DefaultPoses()
		{
			if (_hand == Hand.LEFT ? _handPoseController.defaultLeftPose != null : _handPoseController.defaultRightPose != null) return;

			Transform handTarget = _hand == Hand.LEFT ? _vrIK.references.leftHand : _vrIK.references.rightHand;
			Transform newPoseRoot = CreateNewPoseObject(handTarget);
			newPoseRoot.name = (_hand == Hand.LEFT ? "Left" : "Right") + "HandDefaultPose";
			if (_hand == Hand.LEFT) _handPoseController.defaultLeftPose = newPoseRoot;
			else _handPoseController.defaultRightPose = newPoseRoot;
			EditorUtility.SetDirty(_handPoseController);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		private GameObject CreateNewPoseObject(GameObject handTarget)
		{
			if (handTarget == null) return null;
			Transform returnTrans = CreateNewPoseObject(handTarget.transform);
			if (returnTrans != null) return returnTrans.gameObject;
			return null;
		}

		private Transform CreateNewPoseObject(Transform handTarget)
		{
			if (handTarget == null) return null;
			Transform[] handTransforms = handTarget.GetComponentsInChildren<Transform>();
			Dictionary<Transform, Transform> parentHierarchy = new Dictionary<Transform, Transform>();
			Transform newPoseRoot = null;
			foreach(Transform handTrans in handTransforms)
			{
				GameObject newHandObj = new GameObject(handTrans.name);
				if (handTrans == handTarget)
				{
					newHandObj.transform.parent = _vrIK.transform.parent;
					newHandObj.transform.localPosition = newHandObj.transform.parent.InverseTransformPoint(handTarget.position);
					newHandObj.transform.localRotation = handTarget.rotation;
					newPoseRoot = newHandObj.transform;
				} else
				{
					Transform newParent = null;
					parentHierarchy.TryGetValue(handTrans.parent, out newParent);
					if (newParent == null)
					{
						Debug.LogError("Parent not in parent hiearchy " + handTrans, handTrans.gameObject);
						continue;
					}
					newHandObj.transform.parent = newParent;
					newHandObj.transform.localPosition = handTrans.localPosition;
					newHandObj.transform.localRotation = handTrans.localRotation;
				}
				parentHierarchy.Add(handTrans, newHandObj.transform);
			}
			return newPoseRoot;
		}
	}
}
#endif
