using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRInteraction
{
	public class RigidbodyMarker : MonoBehaviour 
	{
		public bool markToDestroy;

		public float mass;
		public float drag;
		public float angularDrag;
		public bool useGravity;
		public bool isKinematic;
		public RigidbodyInterpolation interpolate;
		public CollisionDetectionMode collisionDetection;
		public RigidbodyConstraints constraints;

		virtual public Rigidbody ReplaceMarkerWithRigidbody()
		{
			Rigidbody body = GetComponent<Rigidbody>();
			if (body == null) body = gameObject.AddComponent<Rigidbody>();
			body.mass = mass;
			body.drag = drag;
			body.angularDrag = angularDrag;
			body.useGravity = useGravity;
			body.isKinematic = isKinematic;
			body.interpolation = interpolate;
			body.collisionDetectionMode = collisionDetection;
			body.constraints = constraints;
			markToDestroy = true;
			Destroy(this);
			return body;
		}

		static public RigidbodyMarker ReplaceRigidbodyWithMarker(Rigidbody body)
		{
			body.isKinematic = true;
			
			RigidbodyMarker marker = body.GetComponent<RigidbodyMarker>();
			if (marker == null || marker.markToDestroy) marker = body.gameObject.AddComponent<RigidbodyMarker>();
			marker.mass = body.mass;
			marker.drag = body.drag;
			marker.angularDrag = body.angularDrag;
			marker.useGravity = body.useGravity;
			marker.isKinematic = body.isKinematic;
			marker.interpolate = body.interpolation;
			marker.collisionDetection = body.collisionDetectionMode;
			marker.constraints = body.constraints;
			Destroy(body);
			return marker;
		}
	}
}