//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Collection of useful static methods
//
//===================Contact Email: Sam@MassGames.co.uk===========================

using UnityEngine;
using System.Collections;

namespace VRInteraction
{

	public class VRUtils : MonoBehaviour 
	{
		public static Vector3 ClosestPointOnLine(Vector3 startPoint, Vector3 endPoint, Vector3 tPoint)
		{
			Vector3 startPointTotPointVector = tPoint - startPoint;
			Vector3 startPointToEndPointVector = (endPoint - startPoint).normalized;

			float d = Vector3.Distance(startPoint, endPoint);
			float t = Vector3.Dot(startPointToEndPointVector, startPointTotPointVector);

			if (t <= 0) 
				return startPoint;

			if (t >= d) 
				return endPoint;

			Vector3 distanceAlongVector = startPointToEndPointVector * t;

			Vector3 closestPoint = startPoint + distanceAlongVector;

			return closestPoint;
		}

		public static float TPositionBetweenPoints(Vector3 startPoint, Vector3 endPoint, Vector3 tPoint)
		{
			Vector3 startPointTotPointVector = tPoint - startPoint;
			Vector3 startPointToEndPointVector = (endPoint - startPoint).normalized;

			float d = Vector3.Distance(startPoint, endPoint);
			float tDist = Vector3.Distance(startPoint, tPoint);
			float t = Vector3.Dot(startPointToEndPointVector, startPointTotPointVector);
			float tPos = tDist / d;

			if (t <= 0f || tPos <= 0f) 
				return 0f;

			if (t >= d || tPos >= 1f) 
				return 1f;

			return tPos;
		}

		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy as T;
		}

		public static Vector3 GetConeDirection(Vector3 originalDirection, float coneSize)
		{
			// random tilt around the up axis
			Quaternion randomTilt = Quaternion.AngleAxis(Random.Range(0f, coneSize), Vector3.up);
			// random spin around the forward axis
			Quaternion randomSpin = Quaternion.AngleAxis(Random.Range(0f, 360f), originalDirection);
			// tilt then spin
			Quaternion tiltSpin = (randomSpin * randomTilt);
			// fire in direction with offset
			Vector3 coneDirection = (tiltSpin * originalDirection).normalized;
			return coneDirection;
		}

		public static bool PositionWithinCone(Vector3 startPosition, Vector3 direction, Vector3 targetPosition, float coneSize, float distance)
		{
			if (Vector3.Distance(startPosition, targetPosition) > distance) return false;
			float cone = Mathf.Cos(coneSize*Mathf.Deg2Rad);
			Vector3 heading = (startPosition - targetPosition).normalized;
			float dot = Vector3.Dot(-direction.normalized, heading);
			return dot > cone;
		}

		public enum BlendMode
		{
			Opaque,
			Cutout,
			Fade,
			Transparent
		}

		public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
		{
			switch (blendMode)
			{
			case BlendMode.Opaque:
				standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				standardShaderMaterial.SetInt("_ZWrite", 1);
				standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = -1;
				break;
			case BlendMode.Cutout:
				standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				standardShaderMaterial.SetInt("_ZWrite", 1);
				standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 2450;
				break;
			case BlendMode.Fade:
				standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				standardShaderMaterial.SetInt("_ZWrite", 0);
				standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
				standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
				standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 3000;
				break;
			case BlendMode.Transparent:
				standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				standardShaderMaterial.SetInt("_ZWrite", 0);
				standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
				standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
				standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				standardShaderMaterial.renderQueue = 3000;
				break;
			}
		}

		///Transforms position from local space to world space, but you can put the position, rotation and scale of the transform in seperately.
		///Useful if it's the position of one transform but the rotation of another.
		static public Vector3 TransformPoint(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 localPosition)
		{
			var localToWorldMatrix = Matrix4x4.TRS(position, rotation, scale);
			return localToWorldMatrix.MultiplyPoint3x4(localPosition);
		}

		///Transforms position from world space to local space, but you can put the position, rotation and scale of the transform in seperately.
		///Useful if it's the position of one transform but the rotation of another.
		static public Vector3 InverseTransformPoint(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 worldPosition)
		{
			var worldToLocalMatrix = Matrix4x4.TRS(position, rotation, scale).inverse;
			return worldToLocalMatrix.MultiplyPoint3x4(worldPosition);
		}

		///Transforms direction from local space to world space, but you can put the rotation of the transform in directly.
		///Useful if you don't want to make the actual gameobject and just want to convert
		static public Vector3 TransformDirection(Quaternion rotation, Vector3 localDirection)
		{
			return rotation * localDirection;
		}

		static public LayerMask StringArrayToLayerMask(string[] layers, bool inverted)
		{
			if (layers == null || layers.Length == 0) return ~0;

			LayerMask layerMask = 1 << LayerMask.NameToLayer(layers[0]);
			for (int i=1 ; i<layers.Length ; i++)
			{
				layerMask |= 1 << LayerMask.NameToLayer(layers[i]);
			}
			if (inverted) layerMask = ~layerMask;
			return layerMask;
		}
	}
}
