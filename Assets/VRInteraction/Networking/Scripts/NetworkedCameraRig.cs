using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRInteraction;

public class NetworkedCameraRig : NetworkBehaviour 
{
	static private List<NetworkedCameraRig> _networkedRigs;
	static public List<NetworkedCameraRig> networkedRigs
	{
		get
		{
			if (_networkedRigs == null) _networkedRigs = new List<NetworkedCameraRig>();
			return _networkedRigs; 
		}
	}

	protected GameObject leftHand;
	protected GameObject rightHand;
	protected PlayerConnectionObject _connection;
	protected bool remoteRig;

	public PlayerConnectionObject connection
	{
		get { return _connection; }
	}

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		networkedRigs.Add(this);
		Camera[] cameras = GetComponentsInChildren<Camera>(true);
		foreach(Camera cam in cameras) cam.enabled = false;

		if (_connection == null) FindMyConnectionObject();
		if (_connection == null) ConvertToRemoteRig();
	}
	
	private void FindMyConnectionObject()
	{
		GameObject[] connectionObjects = GameObject.FindGameObjectsWithTag("ConnectionObject");
		foreach(GameObject connectionObject in connectionObjects)
		{
			PlayerConnectionObject connection = connectionObject.GetComponent<PlayerConnectionObject>();
			if (connection == null) continue;
			if (connection.isLocalPlayer) 
			{
				_connection = connection;
				break;
			}
		}
	}

	public override void OnStartAuthority ()
	{
		base.OnStartAuthority ();
		ConvertOtherPlayerRigsToRemote();

		Camera[] cameras = GetComponentsInChildren<Camera>(true);
		foreach(Camera cam in cameras) cam.enabled = true;

		if (_connection == null) FindMyConnectionObject();

		#if Int_SteamVR

		SteamVR_Camera steamCam = GetComponentInChildren<SteamVR_Camera>();
		if (steamCam != null) steamCam.enabled = true;

		#endif

		VRInput[] inputs = GetComponentsInChildren<VRInput>(true);
		foreach(VRInput input in inputs)
		{
			InputReceivedForwarder forwarder = input.gameObject.AddComponent<InputReceivedForwarder>();
			forwarder.networkRig = this;
			if (input.LeftHand) leftHand = input.gameObject;
			else rightHand = input.gameObject;
		}
	}

	/*IEnumerator ConvertToRemoteIfNewJoiningPlayer()
	{
		yield return new WaitForSeconds(0.1f);
		if (!hasAuthority)
		{
			ConvertToRemoteRig();
		}
	}*/

	void ConvertOtherPlayerRigsToRemote()
	{
		foreach(NetworkedCameraRig networkedRig in networkedRigs)
		{
			if (networkedRig == null || networkedRig == this) continue;
			networkedRig.ConvertToRemoteRig();
		}
	}

	/// <summary>
	/// Strip the SteamVR or Oculus camera rig and add the remote scripts
	/// for receiving input.
	/// </summary>
	public void ConvertToRemoteRig()
	{
		if (remoteRig) return;

		remoteRig = true;
		Component[] components = GetComponentsInChildren<Component>(true);
		foreach(Component comp in components)
		{
			if (comp == null || !CanDestroyType(comp) || !comp.gameObject.CanDestroy(comp.GetType())) continue;

			if (comp.GetType() == typeof(VRInput))
			{
				VRInputRemote inputRemote = comp.gameObject.AddComponent<VRInputRemote>();
				// Initial Sync
				VRInput originalInput = (VRInput)comp;
				inputRemote.VRActions = originalInput.VRActions;
				inputRemote._isSteamVR = originalInput.isSteamVR();
				inputRemote._hmdType = originalInput.hmdType;
				inputRemote._leftHand = originalInput.LeftHand;
				if (originalInput.LeftHand) leftHand = comp.gameObject;
				else rightHand = comp.gameObject;

				inputRemote.triggerKey = originalInput.triggerKey;
				inputRemote.padTop = originalInput.padTop;
				inputRemote.padLeft = originalInput.padLeft;
				inputRemote.padRight = originalInput.padRight;
				inputRemote.padBottom = originalInput.padBottom;
				inputRemote.padCentre = originalInput.padCentre;
				inputRemote.padTouch = originalInput.padTouch;
				inputRemote.gripKey = originalInput.gripKey;
				inputRemote.menuKey = originalInput.menuKey;
				inputRemote.AXKey = originalInput.AXKey;
				inputRemote.triggerKeyOculus = originalInput.triggerKeyOculus;
				inputRemote.padTopOculus = originalInput.padTopOculus;
				inputRemote.padLeftOculus = originalInput.padLeftOculus;
				inputRemote.padRightOculus = originalInput.padRightOculus;
				inputRemote.padBottomOculus = originalInput.padBottomOculus;
				inputRemote.padCentreOculus = originalInput.padCentreOculus;
				inputRemote.padTouchOculus = originalInput.padTouchOculus;
				inputRemote.gripKeyOculus = originalInput.gripKeyOculus;
				inputRemote.menuKeyOculus = originalInput.menuKeyOculus;
				inputRemote.AXKeyOculus = originalInput.AXKeyOculus;
			}

			if (comp.GetType() == typeof(VRInteractor))
			{
				VRInteractorRemote interactorRemote = comp.gameObject.AddComponent<VRInteractorRemote>();
				//Initial Sync
				VRInteractor originalInteractor = (VRInteractor)comp;
				interactorRemote.useHoverLine = originalInteractor.useHoverLine;
				if (originalInteractor.useHoverLine) interactorRemote.hoverLineMat = originalInteractor.hoverLineMat;

				interactorRemote.hideControllersWhileHolding = originalInteractor.hideControllersWhileHolding;
				interactorRemote.controllerAnchor = originalInteractor.controllerAnchor;
				interactorRemote.controllerAnchorOffset = originalInteractor.controllerAnchorOffset;
				interactorRemote.ikTarget = originalInteractor.ikTarget;
				interactorRemote.forceGrabDirection = originalInteractor.forceGrabDirection;
				interactorRemote.forceGrabDistance = originalInteractor.forceGrabDistance;
			}

			Destroy(comp);
		}
		foreach(Component comp in components)
		{
			if (comp == null || !CanDestroyType(comp)) continue;
			Destroy(comp);
		}
		if (leftHand != null) leftHand.SetActive(true);
		if (rightHand != null) rightHand.SetActive(true);
	}

	/// <summary>
	/// Called by InputReceivedForward.cs on the remote prefab.
	/// This should only be call on the localplayer machine.
	/// Tell's server to probagate input to connected remotes
	/// </summary>
	/// <param name="param">Parameter.</param>
	public void InputReceived(string method, bool isLeftHand)
	{
		CmdInputReceived(method, isLeftHand);
	}

	public void InputDirty(VRInput input)
	{
		CmdInputDirty(input.TriggerPressed, input.TriggerPressure, input.PadTopPressed,
			input.PadLeftPressed, input.PadRightPressed, input.PadBottomPressed,
			input.PadCentrePressed, input.PadTouched, input.PadPressed, input.PadPosition,
			input.GripPressed, input.MenuPressed, input.AXPressed, input.LeftHand);
	}

	public void InteractorDirty(VRInteractor interactor, bool isLeftHand)
	{
		CmdInteractorDirty(interactor.Velocity, interactor.AngularVelocity, isLeftHand);
	}

	[Command]
	void CmdInputReceived(string method, bool isLeftHand)
	{
		RpcInputReceived(method, isLeftHand);
	}

	[Command]
	void CmdInputDirty(bool triggerPressed, float triggerPressure, bool padTopPressed,
						bool padLeftPressed, bool padRightPressed, bool padBottomPressed,
						bool padCentrePressed, bool padTouched, bool padPressed,
						Vector2 padPosition, bool gripPressed, bool menuPressed,
						bool axPressed, bool isLeftHand)
	{
		RpcInputDirty(triggerPressed, triggerPressure, padTopPressed,
			padLeftPressed, padRightPressed, padBottomPressed, padCentrePressed,
			padTouched, padPressed, padPosition, gripPressed, menuPressed,
			axPressed, isLeftHand);
	}

	[Command]
	void CmdInteractorDirty(Vector3 velocity, Vector3 angularVelocity, bool isLeftHand)
	{
		RpcInteractorDirty(velocity, angularVelocity, isLeftHand);
	}

	[ClientRpc]
	void RpcInputReceived(string method, bool isLeftHand)
	{
		Debug.LogError("Received input " + hasAuthority);
		if (hasAuthority) return;
		GameObject targetHand = isLeftHand ? leftHand : rightHand;
		Debug.LogError("has target hand " + targetHand);
		if (targetHand == null) return;
		Debug.LogError("sending input to hand");
		targetHand.SendMessage("InputReceived", method);
	}

	[ClientRpc]
	void RpcInputDirty(bool triggerPressed, float triggerPressure, bool padTopPressed, 
						bool padLeftPressed, bool padRightPressed, bool padBottomPressed,
						bool padCentrePressed, bool padTouched, bool padPressed,
						Vector2 padPosition, bool gripPressed, bool menuPressed,
						bool axPressed,  bool isLeftHand)
	{
		if (hasAuthority) return;
		GameObject targetObject = isLeftHand ? leftHand : rightHand;
		if (targetObject == null) return;
		VRInputRemote input = targetObject.GetComponent<VRInputRemote>();
		if (input == null) return;
		input._triggerPressed = triggerPressed;
		input._triggerPressure = triggerPressure;
		input._padTopPressed = padTopPressed;
		input._padLeftPressed = padLeftPressed;
		input._padRightPressed = padRightPressed;
		input._padBottomPressed = padBottomPressed;
		input._padCentrePressed = padCentrePressed;
		input._padTouched = padTouched;
		input._padPressed = padPressed;
		input._padPosition = padPosition;
		input._gripPressed = gripPressed;
		input._menuPressed = menuPressed;
		input._axPressed = axPressed;
	}

	[ClientRpc]
	void RpcInteractorDirty(Vector3 velocity, Vector3 angularVelocity, bool isLeftHand)
	{
		if (hasAuthority) return;
		GameObject targetObject = isLeftHand ? leftHand : rightHand;
		if (targetObject == null) return;
		VRInteractorRemote interactor = targetObject.GetComponent<VRInteractorRemote>();
		if (interactor == null) return;
		interactor._velocity = velocity;
		interactor._angularVelocity = angularVelocity;
	}

	virtual protected bool CanDestroyType(Component comp)
	{
		if (comp == null) return false;
		if (comp.GetType() == typeof(Transform) ||
			comp.GetType() == typeof(NetworkIdentity) ||
			comp.GetType() == typeof(NetworkTransform) ||
			comp.GetType() == typeof(NetworkTransformChild) ||
			comp.GetType().IsSubclassOf(typeof(NetworkedCameraRig)) ||
			comp.GetType() == typeof(NetworkedCameraRig) ||
			comp.GetType() == typeof(VRInteractorRemote) ||
			comp.GetType() == typeof(VRInputRemote) ||
			comp.GetType() == typeof(ForceGrabToggle) ||
			comp.GetType() == typeof(MeshFilter) ||
			comp.GetType() == typeof(MeshRenderer) ||
			comp.GetType() == typeof(SkinnedMeshRenderer) ||
			comp.GetType() == typeof(Animator))
			return false;
		return true;
	}

	public void LocalAssignAuthority(GameObject targetItem)
	{
		if (connection != null && connection.isLocalPlayer)
		{
			CmdAssignAuthority(targetItem.GetComponent<NetworkIdentity>(), connection.GetComponent<NetworkIdentity>());
		}
	}

	public void LocalRemoveAuthority(GameObject targetItem)
	{
		if (connection != null && connection.isLocalPlayer)
		{
			CmdRemoveAuthority(targetItem.GetComponent<NetworkIdentity>(), connection.GetComponent<NetworkIdentity>());
		}
	}

	[Command]
	private void CmdAssignAuthority(NetworkIdentity targetItem, NetworkIdentity targetPlayer)
	{
		targetItem.AssignClientAuthority(targetPlayer.connectionToClient);
	}

	[Command]
	private void CmdRemoveAuthority(NetworkIdentity targetItem, NetworkIdentity targetPlayer)
	{
		targetItem.RemoveClientAuthority(targetPlayer.connectionToClient);
	}
}
