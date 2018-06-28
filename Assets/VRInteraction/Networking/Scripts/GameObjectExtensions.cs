using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

static class GameObjectExtensions
{
	private static bool Requires(Type obj, Type requirement)
	{
		return Attribute.IsDefined(obj, typeof(RequireComponent)) &&
			Attribute.GetCustomAttributes(obj, typeof(RequireComponent)).OfType<RequireComponent>()
				.Any(rc => rc == null || rc.m_Type0.IsAssignableFrom(requirement));
	}

	internal static bool CanDestroy(this GameObject go, Type t)
	{
		if (go == null) return false;
		return !go.GetComponents<Component>().Any(c => c == null || Requires(c.GetType(), t));
	}
}
