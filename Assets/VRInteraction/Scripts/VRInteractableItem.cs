//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Base InteractableItem can be picked up and dropped by a VRInteractor
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace VRInteraction
{
	public class VRInteractableItem : MonoBehaviour {

		public enum HoverMode
		{
			SHADER,
			MATERIAL
		}
		public enum HoldType
		{
			FIXED_POSITION,
			PICKUP_POSITION,
			SPRING_JOINT
		}

		static private List<VRInteractableItem> _items;
		static public List<VRInteractableItem> items
		{
			get
			{
				if (_items == null) _items = new List<VRInteractableItem>();
				return _items; 
			}
		}

		//References
		public Transform item;
		public List<Renderer> hovers = new List<Renderer>();
		public List<Collider> triggerColliders = new List<Collider>();
		public Transform leftHandIKAnchor;
		public Transform rightHandIKAnchor;
		public string leftHandIKPoseName;
		public string rightHandIkPoseName;

		//Set parent if this item can't be interacted with unless the parent is being held
		public List<VRInteractableItem> parents = new List<VRInteractableItem>();

		//Variables
		public string itemId;
		public bool canBeHeld = true;
		public bool interactionDisabled = false;
		public HoldType holdType = HoldType.FIXED_POSITION;
		public bool useBreakDistance = false;
		public float breakDistance = 0.1f;
		public bool linkedLeftAndRightHeldPositions = true;
		public Vector3 heldPosition = Vector3.zero;
		public Quaternion heldRotation = Quaternion.identity;
		public Vector3 heldPositionRightHand = Vector3.zero;
		public Quaternion heldRotationRightHand = Quaternion.identity;
		public float throwBoost = 1f;
		public float followForce = 1f;
		public float interactionDistance = 0.1f;
		public bool limitAcceptedAction;
		public List<string> acceptedActions = new List<string>();

		public List<HoverMode> hoverModes = new List<HoverMode>();
		public List<Shader> defaultShaders = new List<Shader>();
		public List<Shader> hoverShaders = new List<Shader>();
		public List<Material> defaultMats = new List<Material>();
		public List<Material> hoverMats = new List<Material>();
		public bool toggleToPickup;
		public UnityEvent pickupEvent;
		public UnityEvent dropEvent;
		public UnityEvent enableHoverEvent;
		public UnityEvent disableHoverEvent;

		//Sounds
		public AudioSource audioSource;
		public AudioClip enterHover;
		public AudioClip exitHover;
		public AudioClip pickupSound;
		public AudioClip dropSound;
		public AudioClip forceGrabSound;

		//Editor Vars
		public bool hoverFoldout;
		public bool triggersFoldout;
		public bool soundsFoldout;
		public bool ikFoldout;

		//protected string originalShaderName;
		protected Rigidbody _selfBody;
		protected Collider itemCollider;
		protected List<SpringJoint> _springJoints = new List<SpringJoint>();
		protected bool activeHover = false;
		protected float currentFollowForce = -1f;
		protected bool _pickingUp;
		protected object[] selfParam;
		protected static int itemIdIndex;

		protected VRInteractor _heldBy;
		protected List<VRInteractor> _heldBys = new List<VRInteractor>();
		public VRInteractor heldBy
		{
			get { return _heldBy; }
			set { _heldBy = value; }
		}

		public Rigidbody selfBody
		{
			get
			{
				if (_selfBody == null) _selfBody = item.GetComponent<Rigidbody>();
				return _selfBody; 
			}
		}

		public bool InteractionDisabled
		{
			get { return interactionDisabled; }
			set { interactionDisabled = value; }
		}

		protected object[] getSelfParam
		{
			get
			{
				if (selfParam == null) selfParam = new object[] {this};
				return selfParam;
			}
		}

		void Start()
		{
			Init();
		}

		virtual protected void OnEnable()
		{
			VRInteractableItem.items.Add(this);
		}

		virtual protected void OnDisable()
		{
			if (heldBy != null)
			{
				var oldCanBeHeld = canBeHeld;
				canBeHeld = false;
				heldBy.Drop();
				canBeHeld = oldCanBeHeld;
			}
			VRInteractableItem.items.Remove(this);
		}

		void FixedUpdate()
		{
			Step();
		}

		virtual protected void Init()
		{
			if (item == null) 
			{
				Debug.LogError("Item object not set for Interactable Item", gameObject);
				return;
			}

			//Initialize self param (This is so it's not being made when the object is being destroyed)
			selfParam = getSelfParam;

			if (string.IsNullOrEmpty(itemId)) itemId = (1000+(itemIdIndex++)).ToString();

			if (item.GetComponent<Rigidbody>() != null)
			{
				Collider[] colliders = item.GetComponentsInChildren<Collider>();
				foreach(Collider col in colliders)
				{
					if (col.isTrigger || col.GetComponent<VRItemCollider>() != null || col.GetComponent<VRInteractableItem>() != null) continue;

					Debug.LogWarning("Non trigger collider without VRItemCollider script. " +
						"if the item is going crazy when you pick it up, this is why. Remove this collider or " +
						"add a VRItemCollider script and reference the interactable item or set is trigger to true.", col.gameObject);
				}
			}

			if (Camera.main != null && Camera.main.transform.parent != null)
			{
				Collider playerRigCollider = Camera.main.transform.parent.GetComponent<Collider>();
				if (playerRigCollider != null && !playerRigCollider.isTrigger)
				{
					Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
					foreach(Collider itemCol in itemColliders)
					{
						if (itemCol.isTrigger) continue;
						Physics.IgnoreCollision(playerRigCollider, itemCol);
					}
				}
			}
			itemCollider = item.GetComponent<Collider>();
			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null)
				{
					Debug.LogError(name + " has a missing renderer. Check the Hover section of the editor", gameObject);
					continue;
				}
				switch(hoverModes[i])
				{
				case HoverMode.SHADER:
					if (hover.material == null) break;
					if (hoverShaders[i] == null) hoverShaders[i] = Shader.Find("Unlit/Texture");
					if (defaultShaders[i] == null) defaultShaders[i] = hover.material.shader;
					else hover.material.shader = defaultShaders[i];
					break;
				case HoverMode.MATERIAL:
					if (hoverMats[i] == null)
					{
						hoverMats[i] = new Material(hover.material);
						hoverMats[i].shader = Shader.Find("Unlit/Texture");
					}
					if (defaultMats[i] == null) defaultMats[i] = hover.material;
					else hover.material = defaultMats[i];
					break;
				}
			}
		}

		virtual protected void Step()
		{
			if (item == null || heldBy == null || interactionDisabled || holdType == HoldType.SPRING_JOINT) return;

			if (useBreakDistance && Vector3.Distance(heldBy.getControllerAnchorOffset.position, GetWorldHeldPosition(heldBy)) > breakDistance)
			{
				heldBy.Drop();
				return;
			}

			if (!canBeHeld) return;

			if (selfBody == null)
			{
				item.position = GetControllerPosition(heldBy);
				item.rotation = GetControllerRotation(heldBy);
				return;
			}
			selfBody.maxAngularVelocity = float.MaxValue;

			Quaternion rotationDelta = GetHeldRotationDelta();
			Vector3 positionDelta = GetHeldPositionDelta();

			float angle;
			Vector3 axis;
			rotationDelta.ToAngleAxis(out angle, out axis);

			if (angle > 180)
				angle -= 360;

			if (currentFollowForce < 0f) currentFollowForce = followForce;

			if (angle != 0)
			{
				Vector3 angularTarget = (angle * axis)*(currentFollowForce*(Time.fixedDeltaTime*100f));
				selfBody.angularVelocity = angularTarget;
			}

			Vector3 velocityTarget = (positionDelta / Time.fixedDeltaTime) * currentFollowForce;

			if (float.IsInfinity(velocityTarget.x) || float.IsNaN(velocityTarget.x))
				velocityTarget = Vector3.zero;
			selfBody.velocity = velocityTarget;
		}

		virtual public bool CanInteract()
		{
			return heldBy == null && !interactionDisabled && (parents.Count == 0 || IsParentItemHeld());
		}

		virtual public bool IsParentItemHeld()
		{
			bool held = false;
			foreach(VRInteractableItem parent in parents)
			{
				if (parent == null) continue;
				if (parent.heldBy != null)
				{
					held = true;
					break;
				}
			}
			return held;
		}

		virtual public bool Pickup(VRInteractor hand)
		{
			if (canBeHeld && item != null)
			{
				switch(holdType)
				{
				case HoldType.FIXED_POSITION:
					item.SetParent(hand.GetVRRigRoot);
					StartCoroutine(PickingUp(hand));
					VRInteractableItem.HeldFreezeItem(item.gameObject);
					break;
				case HoldType.PICKUP_POSITION:
					if (Vector3.Distance(hand.getControllerAnchorOffset.position, item.position) < interactionDistance)
						heldPosition = hand.getControllerAnchorOffset.InverseTransformPoint(item.position);
					else
						heldPosition = Vector3.zero;
					heldRotation = Quaternion.Inverse(hand.getControllerAnchorOffset.rotation) * item.rotation;
					item.SetParent(hand.GetVRRigRoot);
					StartCoroutine(PickingUp(hand));
					VRInteractableItem.HeldFreezeItem(item.gameObject);
					break;
				case HoldType.SPRING_JOINT:
					SpringJoint springJoint = item.gameObject.AddComponent<SpringJoint>();
					Rigidbody controllerBody = hand.getControllerAnchorOffset.GetComponent<Rigidbody>();
					if (controllerBody == null) controllerBody = hand.getControllerAnchorOffset.gameObject.AddComponent<Rigidbody>();
					controllerBody.isKinematic = true;
					controllerBody.useGravity = false;
					springJoint.connectedBody = controllerBody;
					//springJoint.anchor = Vector3.zero;
					springJoint.anchor = item.InverseTransformPoint(hand.getControllerAnchorOffset.position);
					springJoint.autoConfigureConnectedAnchor = false;
					springJoint.connectedAnchor = Vector3.zero;
					springJoint.spring = followForce*100f;
					springJoint.damper = 100f;
					_springJoints.Add(springJoint);
					_heldBys.Add(hand);
					break;
				}

				if (Vector3.Distance(hand.getControllerAnchorOffset.position, item.position) < interactionDistance)
					PlaySound(pickupSound);
				else PlaySound(forceGrabSound, hand.getControllerAnchorOffset.position);
			} else CheckIK(true, hand);
			if (pickupEvent != null) pickupEvent.Invoke();
			heldBy = hand;
			return true;
		}

		virtual protected IEnumerator PickingUp(VRInteractor heldBy)
		{
			currentFollowForce = 0.05f;
			_pickingUp = true;
			float baseDist = Vector3.Distance(GetControllerPosition(heldBy), item.position);
			if (baseDist < 0.1f) baseDist = 0.1f;
			float elapsedTime = 0;
			while(currentFollowForce < 0.99f)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime > baseDist*0.25f) break; //Maximum time safety
				float dist = Vector3.Distance(GetControllerPosition(heldBy), item.position);
				float percent = -((dist / baseDist)-1f);
				if (baseDist > 1f && percent < 0.3f) percent *= 0.2f;
				if (percent < 0.05f) percent = 0.05f;
				currentFollowForce = followForce*percent;
				yield return null;
				if (this.heldBy != heldBy) 
				{
					currentFollowForce = followForce;
					_pickingUp = false;
					break;
				}
			}
			CheckIK(true, heldBy);
			_pickingUp = false;
			currentFollowForce = followForce;
		}

		virtual public void Drop(bool addControllerVelocity, VRInteractor hand = null)
		{
			if (canBeHeld && item != null)
			{
				item.parent = null;
				switch(holdType)
				{
				case HoldType.FIXED_POSITION:
				case HoldType.PICKUP_POSITION:
					VRInteractableItem.UnFreezeItem(item.gameObject);
					if (selfBody != null)
					{
						if (hand != null && addControllerVelocity)
						{
							bool useBoost = hand.Velocity.magnitude > 1f;
							selfBody.velocity = hand.Velocity * (useBoost ? throwBoost : 1f);
							selfBody.angularVelocity = hand.AngularVelocity;
							selfBody.maxAngularVelocity = selfBody.angularVelocity.magnitude;
						}
					}
					break;
				case HoldType.SPRING_JOINT:
					for(int i=_heldBys.Count-1; i>=0; i--)
					{
						if (_heldBys[i] != hand) continue;
						_heldBys.RemoveAt(i);
						Destroy(_springJoints[i]);
						_springJoints.RemoveAt(i);
					}
					Rigidbody controllerBody = hand.getControllerAnchorOffset.GetComponent<Rigidbody>();
					if (controllerBody != null) Destroy(controllerBody);
					break;
				}
				PlaySound(dropSound);
			}
			CheckIK(false, hand);
			if (dropEvent != null) dropEvent.Invoke();
			heldBy = null;
		}

		virtual protected void PICKUP_DROP(VRInteractor hand)
		{
			if (hand.heldItem == null) hand.TryPickup();
			else if (toggleToPickup) hand.Drop();
		}

		virtual protected void PICKUP_DROPReleased(VRInteractor hand)
		{
			if (hand.heldItem == null || toggleToPickup || hand.vrInput.ActionPressed("PICKUP") || hand.vrInput.ActionPressed("ACTION") || hand.vrInput.ActionPressed("PICKUP_DROP")) return;

			hand.Drop();
		}

		virtual protected void PICKUP(VRInteractor hand)
		{
			if (hand.heldItem != null) return;

			hand.TryPickup();
		}

		virtual protected void PICKUPReleased(VRInteractor hand)
		{
			PICKUP_DROPReleased(hand);
		}

		virtual protected void DROP(VRInteractor hand)
		{
			if (hand.heldItem == null) return;

			hand.Drop();
		}

		virtual protected void DROPReleased(VRInteractor hand)
		{}

		virtual protected void ACTION(VRInteractor hand)
		{
			if (hand.heldItem != null) return;

			PICKUP_DROP(hand);
		}

		virtual protected void ACTIONReleased(VRInteractor hand)
		{
			PICKUP_DROPReleased(hand);
		}

		virtual protected void CheckIK(bool pickingUp, VRInteractor hand)
		{
			if (hand == null || hand.ikTarget == null) return;
			if (pickingUp)
			{
				Transform handIKAnchor = hand.vrInput.LeftHand ? leftHandIKAnchor : rightHandIKAnchor;
				if (handIKAnchor != null) hand.SetIKTarget(handIKAnchor);
				if ((hand.vrInput.LeftHand && leftHandIKPoseName != "") ||
					(!hand.vrInput.LeftHand && rightHandIkPoseName != ""))
				{
					//	Method is in HandPoseController.cs, found in the FinalIK integrations folder (make sure to open the FinalIK package in VRInteraction first).
					hand.GetVRRigRoot.BroadcastMessage(hand.vrInput.LeftHand ? "ApplyPoseLeftHand" : "ApplyPoseRightHand", hand.vrInput.LeftHand ? leftHandIKPoseName : rightHandIkPoseName, SendMessageOptions.DontRequireReceiver);
				}
			} else
			{
				hand.SetIKTarget(null);
				if ((hand.vrInput.LeftHand && leftHandIKPoseName != "") ||
					(!hand.vrInput.LeftHand && rightHandIkPoseName != ""))
				{
					//	Method is in HandPoseController.cs, found in the FinalIK integrations folder (make sure to open the FinalIK package in VRInteraction first).
					hand.GetVRRigRoot.BroadcastMessage("ClearPose", hand.vrInput.LeftHand, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		virtual public Vector3 GetControllerPosition(VRInteractor hand)
		{
			return hand.getControllerAnchorOffset.TransformPoint(GetLocalHeldPosition(hand));
		}

		virtual public Quaternion GetControllerRotation(VRInteractor hand)
		{
			return heldBy.getControllerAnchorOffset.rotation * GetLocalHeldRotation(heldBy);
		}

		/// <summary>
		/// Get item held position in world space
		/// </summary>
		/// <returns>The world held position.</returns>
		/// <param name="hand">Hand.</param>
		virtual public Vector3 GetWorldHeldPosition(VRInteractor hand)
		{
			if (item == null) return Vector3.zero;
			switch(holdType)
			{
			case HoldType.FIXED_POSITION:
				return item.position - (item.rotation * (Quaternion.Inverse(GetLocalHeldRotation(hand)) * GetLocalHeldPosition(hand)));
			case HoldType.PICKUP_POSITION:
			case HoldType.SPRING_JOINT:
				return item.position;
			}
			return Vector3.zero;
		}

		/// <summary>
		/// Get item held position in parent tranform local space.
		/// Used on the gun slide to get the controller position as a child of the gunhandler item transform
		/// </summary>
		/// <returns>The local controller position to parent transform.</returns>
		/// <param name="hand">Hand.</param>
		/// <param name="item">Item.</param>
		/// <param name="parent">Parent.</param>
		public static Vector3 GetLocalControllerPositionToParentTransform(VRInteractor hand, VRInteractableItem item, Transform parent)
		{
			Vector3 controllerPosition = item.GetControllerPosition(hand);
			return parent.InverseTransformPoint(controllerPosition);
		}

		/*public Vector3 ControllerPositionToLocal()
		{
			if (heldBy == null || currentGun == null) return Vector3.zero;
			Vector3 localPosition =  currentGun.item.InverseTransformPoint(heldBy.getControllerAnchorOffset.position);
			Vector3 rotatedOffset = Quaternion.Inverse(GetLocalHeldRotation(heldBy)) * defaultRotation * GetLocalHeldPosition(heldBy);
			Vector3 scaledOffset = new Vector3(rotatedOffset.x/currentGun.item.localScale.x, rotatedOffset.y/currentGun.item.localScale.y, rotatedOffset.z/currentGun.item.localScale.z);
			return localPosition + scaledOffset;
		}*/

		//virtual public Vector3 GetControllerPositionItemAligned(VRInteractor hand)
		//{
		//	return VRUtils.TransformPoint(hand.getControllerAnchorOffset.position, item.rotation, hand.getControllerAnchorOffset.lossyScale, GetLocalHeldPosition(hand));
		//}

		virtual public Vector3 GetLocalHeldPosition(VRInteractor hand)
		{
			if (linkedLeftAndRightHeldPositions || hand.vrInput.LeftHand)
					return heldPosition;
			else if (!linkedLeftAndRightHeldPositions && !hand.vrInput.LeftHand)
					return heldPositionRightHand;
			
			Debug.LogError("No held position. LinkedLeftAndRightHeldPositions: " + linkedLeftAndRightHeldPositions + " hand.LeftHand: " + hand.vrInput.LeftHand);
			return Vector3.zero;
		}

		virtual public Quaternion GetLocalHeldRotation(VRInteractor hand)
		{
			if (linkedLeftAndRightHeldPositions || hand.vrInput.LeftHand)
					return heldRotation;
			else if (!linkedLeftAndRightHeldPositions && !hand.vrInput.LeftHand)
					return heldRotationRightHand;

			Debug.LogError("No held rotation. LinkedLeftAndRightHeldPositions: " + linkedLeftAndRightHeldPositions + " hand.LeftHand: " + hand.vrInput.LeftHand);
			return Quaternion.identity;
		}

		virtual protected Vector3 GetHeldPositionDelta()
		{
			Transform heldByTransform = heldBy.getControllerAnchorOffset;
			return (heldByTransform.TransformPoint(GetLocalHeldPosition(heldBy))) - item.position;
		}

		virtual protected Quaternion GetHeldRotationDelta()
		{
			Transform heldByTransform = heldBy.getControllerAnchorOffset;
			return (heldByTransform.rotation*GetLocalHeldRotation(heldBy)) * Quaternion.Inverse(item.rotation);
		}
			
		virtual public void EnableHover(VRInteractor hand = null)
		{
			if (activeHover || interactionDisabled) return;
			activeHover = true;
			PlaySound(enterHover);
			if (enableHoverEvent != null) enableHoverEvent.Invoke();
			if (hovers.Count == 0) return;
			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null) continue;
				switch(hoverModes[i])
				{
				case HoverMode.SHADER:
					if (hover.material != null)
						hover.material.shader = hoverShaders[i];
					break;
				case HoverMode.MATERIAL:
					hover.material = hoverMats[i];
					break;
				}
			}
		}

		virtual public void DisableHover(VRInteractor hand = null)
		{
			if (!activeHover || interactionDisabled) return;
			activeHover = false;
			PlaySound(exitHover);
			if (disableHoverEvent != null) disableHoverEvent.Invoke();
			if (hovers.Count == 0) return;
			for(int i=0; i<hovers.Count; i++)
			{
				Renderer hover = hovers[i];
				if (hover == null) continue;
				switch(hoverModes[i])
				{
				case HoverMode.SHADER:
					if (hover.material != null)
						hover.material.shader = defaultShaders[i];
					break;
				case HoverMode.MATERIAL:
					hover.material = defaultMats[i];
					break;
				}
			}
		}

		//Set item up to be held correctly
		static public void HeldFreezeItem(GameObject item)
		{
			Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
			foreach(Collider col in itemColliders)
			{
				VRInteractableItem ii = null;
				VRItemCollider ic = col.GetComponent<VRItemCollider>();
				if (ic != null) ii = ic.item;
				if (ii == null) ii = col.GetComponent<VRInteractableItem>();
				if (ii != null && (ii.parents.Count != 0 || !ii.enabled || ii.interactionDisabled)) continue;
				col.enabled = true;
			}
			RigidbodyMarker bodyMarker = item.GetComponent<RigidbodyMarker>();
			if (bodyMarker != null) 
			{
				Rigidbody body = bodyMarker.ReplaceMarkerWithRigidbody();
				body.isKinematic = false;
			} else
			{
				Rigidbody body = item.GetComponent<Rigidbody>();
				if (body != null) body.isKinematic = false;
			}
		}

		//Disable unity physics and collision. For moving item through code
		static public void FreezeItem(GameObject item, bool disableAllColliders = false, bool disableTriggerColliders = false, bool disableNonTriggerColliders = false)
		{
			VRInteractableItem.DisableObjectColliders(item, disableAllColliders, disableTriggerColliders, disableNonTriggerColliders);
			Rigidbody itemBody = item.GetComponent<Rigidbody>();
			if (itemBody != null) RigidbodyMarker.ReplaceRigidbodyWithMarker(itemBody);
		}

		//Enable unity physics
		static public void UnFreezeItem(GameObject item)
		{
			Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
			foreach(Collider col in itemColliders)
			{
				VRInteractableItem ii = null;
				VRItemCollider ic = col.GetComponent<VRItemCollider>();
				if (ic != null) ii = ic.item;
				if (ii == null) ii = col.GetComponent<VRInteractableItem>();
				if (ii != null && (ii.parents.Count != 0 || !ii.enabled || ii.interactionDisabled)) continue;
				col.enabled = true;
			}
			RigidbodyMarker bodyMarker = item.GetComponent<RigidbodyMarker>();
			if (bodyMarker != null)
			{
				Rigidbody body = bodyMarker.ReplaceMarkerWithRigidbody();
				body.isKinematic = false;
			} else
			{
				Rigidbody body = item.GetComponent<Rigidbody>();
				if (body != null) body.isKinematic = false;
			}
			/*Rigidbody itemBody = item.GetComponentInChildren<Rigidbody>();
			if (itemBody != null)
			{
				itemBody.useGravity = true;
				itemBody.isKinematic = false;
				itemBody.constraints = RigidbodyConstraints.None;
				itemBody.interpolation = RigidbodyInterpolation.Interpolate;
			}*/
		}

		static public void DisableObjectColliders(GameObject item, bool disableAllColliders = false, bool disableTriggerColliders = false, bool disableNonTriggerColliders = false)
		{
			Collider[] itemColliders = GetCollidersOf(item, disableAllColliders, disableTriggerColliders, disableNonTriggerColliders);
			ToggleColliders(itemColliders, false);
		}

		static public void EnableObjectColliders(GameObject item, bool enableAllColliders = false, bool enableTriggerColliders = false, bool enableNonTriggerColliders = false)
		{
			Collider[] itemColliders = GetCollidersOf(item, enableAllColliders, enableTriggerColliders, enableNonTriggerColliders);
			ToggleColliders(itemColliders, true);
		}

		static private Collider[] GetCollidersOf(GameObject item, bool all, bool triggers, bool nonTriggers)
		{
			Collider[] itemColliders = item.GetComponentsInChildren<Collider>();
			if (!all)
			{
				itemColliders = item.GetComponentsInChildren<Collider>();
				for(int i=itemColliders.Length-1; i>=0; i--)
				{
					if (itemColliders[i].isTrigger && !triggers ||
						!itemColliders[i].isTrigger && !nonTriggers)
					{
						itemColliders[i] = null;
					}
				}
			}
			return itemColliders;
		}

		static private void ToggleColliders(Collider[] cols, bool toggle)
		{
			if (cols == null) return;
			foreach(Collider col in cols)
			{
				if (col == null) continue;
				col.enabled = toggle;
			}
		}

		public bool CanAcceptMethod(string method)
		{
			if (!limitAcceptedAction) return true;

			foreach(string acceptedAction in acceptedActions)
			{
				if (method != acceptedAction) continue;
				return true;
			}
			return false;
		}

		virtual public void Reset()
		{
			interactionDisabled = false;
			if (item != null) VRInteractableItem.UnFreezeItem(item.gameObject);
		}

		/// <summary>
		/// Gets any connected VRInteractableItem.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="target">Target.</param>
		static public VRInteractableItem GetItem(GameObject target)
		{
			if (target == null) return null;
			VRInteractableItem ii = target.GetComponent<VRInteractableItem>();
			if (ii != null) return ii;

			//If the item isn't on the target it could have an itemCollider that would reference the item
			VRItemCollider itemCollider = target.GetComponent<VRItemCollider>();
			if (itemCollider != null && itemCollider.col.isTrigger) ii = itemCollider.item;
			if (ii != null) return ii;

			return null;
		}

		public void PlaySound(AudioClip clip)
		{
			if (clip == null) return;
			PlaySound(clip, item.position);
		}

		public void PlaySound(AudioClip clip, Vector3 worldPosition)
		{
			if (clip == null) return;
			if (audioSource != null) 
			{
				audioSource.clip = clip;
				audioSource.Play();
			} else
				AudioSource.PlayClipAtPoint(clip, worldPosition);
		}
	}

}
