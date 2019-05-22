#if Int_FinalIK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class HandPoseController : MonoBehaviour 
{
	public Transform defaultLeftPose;
	public Transform defaultRightPose;
	public List<Transform> poses = new List<Transform>();

	private VRIK _vrIk;

	public void ApplyPoseLeftHand(string poseName)
	{
		ApplyPose(true, poseName);
	}

	public void ApplyPoseRightHand(string poseName)
	{
		ApplyPose(false, poseName);
	}

	public void ApplyPose(bool leftHand, string poseName)
	{
		if (_vrIk == null) _vrIk = GetComponent<VRIK>();
		if (_vrIk == null)
		{
			Debug.LogError("This script requires a VRIK component on the same object", gameObject);
			return;
		}
		Transform pose = GetPoseByName(poseName);
		Transform targetHand = leftHand ? _vrIk.references.leftHand : _vrIk.references.rightHand;
		HandPoser handPoser = targetHand.GetComponent<HandPoser>();
		if (handPoser == null) handPoser = targetHand.gameObject.AddComponent<HandPoser>();
		else handPoser.weight = 1f;
		handPoser.poseRoot = pose;
	}

	public void ClearPose(bool leftHand)
	{
		if (_vrIk == null) _vrIk = GetComponent<VRIK>();
		if (_vrIk == null)
		{
			Debug.LogError("This script requires a VRIK component on the same object", gameObject);
			return;
		}
		Transform targetHand = leftHand ? _vrIk.references.leftHand : _vrIk.references.rightHand;
		HandPoser handPoser = targetHand.GetComponent<HandPoser>();
		if (handPoser != null) handPoser.weight = 0f;
	}

	public Transform GetPoseByName(string poseName)
	{
		if (string.IsNullOrEmpty(poseName)) return null;
		foreach(Transform pose in poses)
		{
			if (poseName != pose.name) continue;
			return pose;
		}
		return null;
	}
}
#endif
