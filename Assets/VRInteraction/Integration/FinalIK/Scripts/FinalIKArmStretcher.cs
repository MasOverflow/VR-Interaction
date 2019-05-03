#if Int_FinalIK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

[RequireComponent(typeof(VRIK))]
public class FinalIKArmStretcher : MonoBehaviour 
{
	[Tooltip("Arm length in unity units of the character model. Not your real life arm length.")]
	public float armLength = 0.5f;
	[Tooltip("How much the arms will stretch after moving beyond normal arm length")]
	public float stretchMultiplier = 1.5f;
	[Tooltip("How often it will check and update the arm length. For optimization")]
	public float updateTime = 0.01f;

	private VRIK vrIK;
	private float elapsedTime;

	void Start () 
	{
		vrIK = GetComponent<VRIK>();
	}

	void Update () 
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > updateTime)
		{
			elapsedTime = 0f;
			vrIK.solver.leftArm.armLengthMlp = GetArmLength(vrIK.references.leftShoulder, vrIK.references.leftHand);
			vrIK.solver.rightArm.armLengthMlp = GetArmLength(vrIK.references.rightShoulder, vrIK.references.rightHand);
		}
	}

	private float GetArmLength(Transform shoulder, Transform hand)
	{
		float dist = Vector3.Distance(shoulder.position, hand.position);
		if (dist < armLength) return 1f;
		return 1f + ((dist-armLength)*stretchMultiplier);
	}
}
#endif